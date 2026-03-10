# LifeSync - Feature Roadmap

Relatório de features sugeridas para cada microserviço, organizadas por valor e complexidade.

---

## Visão Geral da Arquitetura

| Microserviço | Status | Entidades Principais |
|---|---|---|
| **Users** | Funcional | User |
| **TaskManager** | Mais maduro | TaskItem, TaskLabel |
| **Financial** | Funcional | Transaction, Category |
| **Gym** | Funcional | Exercise, Routine, RoutineExercise, TrainingSession, CompletedExercise |
| **Nutrition** | Funcional | Diary, Meal, MealFood, Food, Liquid, LiquidType, DailyProgress |
| **Notification** | Básico | EmailMessage |
| **ApiGateway** | YARP configurado | - |
| **WebApp** | Blazor WASM | Frontend completo |

---

## 1. Users (Autenticação e Usuários)

**Já implementado:** SignUp, SignIn, RefreshToken, GetUser, UpdateProfile, Deactivate/Activate, Roles, Email de boas-vindas via RabbitMQ.

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Foto de perfil / Avatar** | Alto | Média | Upload de imagem para o usuário, exibido em todo o app |
| **Alterar senha** | Alto | Baixa | Endpoint para troca de senha com confirmação da senha antiga |
| **Recuperação de senha (Forgot Password)** | Alto | Média | Fluxo completo com envio de email com reset token |
| **Verificação de email** | Alto | Média | Confirmar email após registro (email verification flow) |
| **Two-Factor Authentication (2FA)** | Médio | Alta | TOTP via app autenticador (Google Authenticator, Authy) |
| **Preferências do usuário** | Alto | Baixa | Timezone, idioma, tema (dark/light), unidades de medida |
| **Histórico de login / Audit log** | Médio | Média | Registrar dispositivo, IP, horário dos últimos logins |
| **Exclusão de conta (LGPD/GDPR)** | Alto | Média | Soft delete completo com anonimização de dados pessoais |
| **OAuth / Login social** | Médio | Alta | Google, Apple, GitHub como providers alternativos |

---

## 2. TaskManager (Gerenciamento de Tarefas)

**Já implementado:** CRUD completo de Tasks e Labels, filtros com paginação, batch operations, due date reminders via RabbitMQ, soft delete, status (Pending/InProgress/Completed), prioridades (Low/Medium/High/Urgent).

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Subtarefas (Subtasks)** | Alto | Média | Tarefas filhas com progresso percentual automático |
| **Tarefas recorrentes** | Alto | Alta | Criar tasks automaticamente (diária/semanal/mensal) via background service |
| **Comentários / Notas na tarefa** | Médio | Baixa | Histórico de anotações com timestamp em cada task |
| **Anexos / Arquivos** | Médio | Média | Upload de arquivos associados a tarefas |
| **Lembretes personalizados** | Alto | Média | Configurar múltiplos lembretes (1h antes, 1 dia antes, etc.) |
| **Histórico de mudanças (Activity Log)** | Médio | Média | Registrar quem alterou o quê e quando (auditoria) |
| **Templates de tarefas** | Médio | Baixa | Salvar e reutilizar modelos de tarefas comuns |
| **Projetos / Agrupamento** | Alto | Alta | Organizar tarefas em projetos com visão kanban |
| **Compartilhamento de tarefas** | Médio | Alta | Atribuir tarefas a outros usuários |
| **Estimativa de tempo** | Baixo | Baixa | Campo de tempo estimado + tempo real gasto |
| **Ordenação manual (sort order)** | Médio | Baixa | Campo de posição para permitir reordenação drag-and-drop |

---

## 3. Financial (Controle Financeiro)

**Já implementado:** CRUD de Transactions e Categories, filtros com paginação, Value Object Money (Amount + Currency), PaymentMethod (Cash, CreditCard, DebitCard, BankTransfer, Pix, Other), TransactionType (Income/Expense), flag IsRecurring. O `ReportsController` existe mas está **vazio**.

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Relatórios financeiros** | **Muito Alto** | Alta | Implementar o ReportsController: resumo mensal, por categoria, receita vs despesa, tendências |
| **Orçamento mensal (Budget)** | Alto | Média | Definir limite de gastos por categoria e acompanhar o consumo |
| **Transações recorrentes automáticas** | Alto | Média | Background service para criar transações baseado no flag IsRecurring |
| **Alertas de gastos** | Alto | Média | Notificar quando atingir X% do orçamento da categoria |
| **Dashboard com gráficos** | Alto | Média | Totais por período, pizza por categoria, line chart de evolução |
| **Contas / Carteiras** | Médio | Média | Separar transações por conta (corrente, poupança, cartão) |
| **Transferências entre contas** | Médio | Média | Movimentar dinheiro entre carteiras |
| **Parcelamento** | Médio | Alta | Dividir despesa em parcelas com tracking individual |
| **Exportar relatórios (CSV/PDF)** | Médio | Baixa | Gerar exportação dos dados financeiros |
| **Meta de economia** | Médio | Média | Definir objetivos financeiros com tracking de progresso |
| **Tags em transações** | Baixo | Baixa | Classificação adicional além de categorias |

---

## 4. Gym (Treinos e Academia)

**Já implementado:** CRUD de Exercises (MuscleGroup, ExerciseType, EquipmentType), Routines com RoutineExercises (sets, reps, rest time, peso recomendado, instruções), TrainingSessions com CompletedExercises, Value Objects (SetCount, RepetitionCount, Weight, RestTime), cálculo de duração.

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Dashboard de progresso** | **Muito Alto** | Alta | Gráficos de evolução de carga, volume total, frequência semanal |
| **Personal Records (PRs)** | Alto | Média | Rastrear recordes pessoais por exercício automaticamente |
| **Histórico de exercícios** | Alto | Média | Evolução de carga/reps de um exercício ao longo do tempo |
| **Timer de descanso** | Alto | Baixa | Cronômetro integrado baseado no RestBetweenSets da rotina (frontend) |
| **Clonagem de sessão** | Médio | Baixa | Repetir um treino anterior como base para um novo |
| **Superset / Circuit training** | Médio | Média | Agrupar exercícios em supersets dentro da rotina |
| **Planejamento semanal** | Alto | Média | Associar rotinas a dias da semana (seg=peito, ter=costas...) |
| **Calendário de treinos** | Alto | Baixa | Visualização mensal dos dias treinados (heatmap no frontend) |
| **Exercícios com vídeo/imagem** | Médio | Baixa | URL de demonstração do exercício |
| **Medidas corporais** | Alto | Média | Tracking de peso, % gordura, circunferências ao longo do tempo |
| **Streak / Gamificação** | Médio | Média | Sequência de dias treinados, badges, conquistas |
| **Templates de rotinas pré-definidas** | Médio | Baixa | Rotinas populares prontas (Push/Pull/Legs, Upper/Lower, etc.) |

---

## 5. Nutrition (Nutrição e Dieta)

**Já implementado:** Diary (diário alimentar por dia), Meals com MealFoods, Food com macros completos (proteína, carboidratos, lipídios, cálcio, magnésio, ferro, sódio, potássio), Liquid tracking com LiquidTypes, DailyProgress com Goals (calorias + líquidos), cálculo de porcentagem de progresso, domain events.

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Banco de alimentos pré-cadastrado** | **Muito Alto** | Média | Importar tabela TACO/USDA com milhares de alimentos |
| **Busca por código de barras** | Alto | Alta | Integração com API externa (Open Food Facts) |
| **Metas de macronutrientes** | Alto | Média | Além de calorias, definir metas de proteína/carbs/gordura |
| **Plano alimentar semanal** | Alto | Média | Montar cardápio semanal com refeições pré-definidas |
| **Refeições favoritas / Templates** | Alto | Baixa | Salvar refeições frequentes para reutilizar rapidamente |
| **Histórico e gráficos nutricionais** | Alto | Média | Evolução semanal/mensal de calorias e macros |
| **Alimentos customizados** | Médio | Baixa | Permitir usuário cadastrar seus próprios alimentos |
| **Receitas** | Médio | Média | Criar receitas compostas com cálculo automático de macros |
| **Alertas de hidratação** | Médio | Média | Lembretes periódicos para beber água via push notification |
| **Snap & Log (foto da refeição)** | Baixo | Alta | Registro rápido com foto (futuro: IA para estimar calorias) |
| **Integração com Gym** | Alto | Alta | Ajustar metas calóricas baseado no treino do dia |

---

## 6. Notification (Notificações)

**Já implementado:** Envio de email básico (EmailMessage), consumer de eventos RabbitMQ para UserRegistered e TaskDueReminder.

### Features Sugeridas

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Push Notifications (mobile/web)** | **Muito Alto** | Alta | Notificações em tempo real via FCM/Web Push |
| **Central de notificações in-app** | Alto | Média | Lista de notificações com status lido/não-lido |
| **Preferências de notificação** | Alto | Média | Escolher quais notificações receber e por qual canal |
| **Templates de email customizáveis** | Médio | Baixa | Sistema de templates HTML para diferentes tipos de email |
| **Notificações agendadas** | Médio | Média | Lembretes configurados pelo usuário (não apenas system events) |
| **Integração multi-canal** | Médio | Alta | SMS, WhatsApp, Telegram além de email |
| **Eventos de todos os serviços** | Alto | Média | Gym (lembrete de treino), Financial (alerta de gastos), Nutrition (meta batida) |

---

## 7. Features Cross-Cutting (Entre Microserviços)

| Feature | Valor | Complexidade | Descrição |
|---|---|---|---|
| **Dashboard unificado (Home)** | **Muito Alto** | Alta | Tela inicial agregando resumo de todos os serviços: tarefas do dia, treino, calorias, saldo |
| **Gamificação global** | Alto | Alta | Sistema de pontos/XP por completar tarefas, treinos, bater metas nutricionais |
| **Relatórios semanais por email** | Alto | Média | Resumo semanal automático: tarefas concluídas, treinos feitos, calorias médias, gastos |
| **Exportação de dados** | Médio | Média | Exportar todos os dados do usuário (LGPD compliance) |
| **Modo offline (PWA)** | Alto | Alta | Cache local com sync quando reconectar |
| **Integração com APIs externas** | Médio | Alta | Webhook/API para integrar com Google Fit, Apple Health, MyFitnessPal |
| **Busca global** | Médio | Média | Buscar tarefas, exercícios, alimentos, transações em um único campo |
| **Rate limiting / Throttling** | Médio | Baixa | Proteção contra abuso no API Gateway (YARP) |
| **Observabilidade** | Alto | Média | Logging centralizado, distributed tracing (OpenTelemetry), health checks avançados |
| **Cache distribuído (Redis)** | Médio | Média | Cache de dados frequentes (alimentos, exercícios) para performance |

---

## Prioridades Recomendadas

### Prioridade 1 — Quick Wins (Baixa complexidade, alto valor)

Estas features podem ser implementadas rapidamente e entregam valor visível imediato:

1. **Implementar `ReportsController` no Financial** — o arquivo já existe vazio, basta criar as queries de agregação
2. **Alterar senha e Forgot Password** no Users — fluxos essenciais que todo app precisa
3. **Preferências do usuário** — timezone, tema, idioma melhoram a experiência
4. **Refeições favoritas** no Nutrition — reutilizar refeições frequentes economiza tempo do usuário
5. **Histórico de exercícios** no Gym — consultar evolução de carga é a feature mais pedida em apps fitness
6. **Templates de email** no Notification — melhorar a apresentação dos emails enviados
7. **Comentários em tarefas** no TaskManager — adicionar contexto sem poluir título/descrição

### Prioridade 2 — Alto Impacto (Média complexidade, muito valor)

Features que diferenciam o app e aumentam engajamento:

1. **Dashboard unificado na Home** — agrega valor visível imediato ao abrir o app
2. **Orçamento mensal** no Financial — controle de gastos é o motivo principal de usar o módulo
3. **Personal Records e Dashboard de progresso** no Gym — motivação para continuar treinando
4. **Metas de macronutrientes** no Nutrition — ir além de calorias é o próximo passo natural
5. **Push Notifications** no Notification — engajamento em tempo real
6. **Subtarefas** no TaskManager — organizar tarefas complexas
7. **Relatórios semanais por email** — cross-cutting que mostra o valor de usar todos os módulos juntos

### Prioridade 3 — Diferenciais (Alta complexidade, alto valor)

Features que transformam o LifeSync em um produto completo:

1. **Gamificação cross-service** — XP, streaks e conquistas por usar todo o ecossistema
2. **Tarefas recorrentes** com background service automático
3. **Integração Gym + Nutrition** — ajustar dieta baseado no treino do dia
4. **Banco de alimentos TACO/USDA** — elimina necessidade de cadastro manual
5. **Projetos com visão Kanban** no TaskManager
6. **PWA com modo offline** — funcionar sem internet
7. **2FA e OAuth** no Users — segurança e conveniência

---

## Métricas de Sucesso Sugeridas

| Métrica | Como medir |
|---|---|
| Engajamento diário | % de usuários que acessam o app por dia |
| Retenção semanal | % de usuários que voltam na semana seguinte |
| Features mais usadas | Contagem de requests por endpoint |
| Completude de metas | % de metas nutricionais/financeiras atingidas |
| Taxa de conclusão de tarefas | Tasks completed / Tasks created |
| Frequência de treinos | Sessões por semana por usuário |

---

> Documento gerado em Fevereiro/2026 com base na análise do código-fonte do repositório LifeSync.
