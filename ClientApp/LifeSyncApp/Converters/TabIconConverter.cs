using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class TabIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Safely extract tabName from the parameter; ensure it's always assigned.
            var tabName = parameter as string ?? string.Empty;

            // If value isn't an int (selected tab), return black icon for the tabName.
            if (value is not int selectedTab)
                return GetBlackIcon(tabName);

            // Map tabName to an index
            var tabIndex = tabName switch
            {
                "financeiro" => 0,
                "academico" => 1,
                "tarefas" => 2,
                "nutricao" => 3,
                "perfil" => 4,
                _ => -1
            };

            // If it's the selected tab, return white icon; otherwise black.
            if (selectedTab == tabIndex)
            {
                return GetWhiteIcon(tabName);
            }

            return GetBlackIcon(tabName);
        }

        private string GetWhiteIcon(string tabName)
        {
            return tabName switch
            {
                "financeiro" => "money_white.svg",
                "academico" => "gym_white.svg",
                "tarefas" => "task_white.svg",
                "nutricao" => "nutrition_white.svg",
                "perfil" => "account_white.svg",
                _ => ""
            };
        }

        private string GetBlackIcon(string tabName)
        {
            return tabName switch
            {
                "financeiro" => "money_black.svg",
                "academico" => "gym_black.svg",
                "tarefas" => "task_black.svg",
                "nutricao" => "nutrition.svg",
                "perfil" => "account_black.svg",
                _ => ""
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
