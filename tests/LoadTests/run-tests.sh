#!/bin/bash
# ============================================================
# LifeSync Load Tests Runner
# ============================================================
# Usage:
#   ./run-tests.sh                          # Run all tests with 'average' profile
#   ./run-tests.sh taskmanager              # Run only taskmanager tests
#   ./run-tests.sh all smoke                # Run all tests with 'smoke' profile
#   ./run-tests.sh gateway stress           # Run gateway tests with 'stress' profile
#   USE_GATEWAY=true ./run-tests.sh all     # Route all tests through the gateway
# ============================================================

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SCENARIOS_DIR="$SCRIPT_DIR/scenarios"
RESULTS_DIR="$SCRIPT_DIR/results"

# Defaults
TARGET="${1:-all}"
PROFILE="${2:-average}"

# Create results directory
mkdir -p "$RESULTS_DIR"

# Timestamp for this run
TIMESTAMP=$(date +%Y%m%d_%H%M%S)

echo "============================================"
echo " LifeSync Load Tests"
echo " Profile: $PROFILE"
echo " Target:  $TARGET"
echo " Time:    $(date)"
echo "============================================"
echo ""

run_test() {
    local name=$1
    local file=$2
    local output="$RESULTS_DIR/${name}_${PROFILE}_${TIMESTAMP}"

    echo ">>> Running: $name ($PROFILE profile)"
    echo "-------------------------------------------"

    k6 run \
        --env K6_PROFILE="$PROFILE" \
        --env USE_GATEWAY="${USE_GATEWAY:-false}" \
        --env TEST_EMAIL="${TEST_EMAIL:-loadtest@test.com}" \
        --env TEST_PASSWORD="${TEST_PASSWORD:-LoadTest@123}" \
        --out json="$output.json" \
        --summary-export="$output_summary.json" \
        "$file" \
        2>&1 | tee "$output.log"

    echo ""
    echo ">>> Completed: $name"
    echo "    Results: $output.log"
    echo "==========================================="
    echo ""
}

if [ "$TARGET" == "all" ]; then
    for test_file in "$SCENARIOS_DIR"/*.test.js; do
        test_name=$(basename "$test_file" .test.js)
        run_test "$test_name" "$test_file"
    done
else
    test_file="$SCENARIOS_DIR/${TARGET}.test.js"
    if [ ! -f "$test_file" ]; then
        echo "Error: Test file not found: $test_file"
        echo "Available tests:"
        ls "$SCENARIOS_DIR"/*.test.js 2>/dev/null | xargs -I{} basename {} .test.js | sed 's/^/  - /'
        exit 1
    fi
    run_test "$TARGET" "$test_file"
fi

echo "============================================"
echo " All tests completed!"
echo " Results saved to: $RESULTS_DIR"
echo "============================================"
