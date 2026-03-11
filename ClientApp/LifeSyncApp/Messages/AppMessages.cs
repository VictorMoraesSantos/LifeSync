using CommunityToolkit.Mvvm.Messaging.Messages;

namespace LifeSyncApp.Messages;

public class SelectTabMessage : ValueChangedMessage<int>
{
    public SelectTabMessage(int tabIndex) : base(tabIndex) { }
}

public class GoBackTabMessage : ValueChangedMessage<bool>
{
    public GoBackTabMessage() : base(true) { }
}

public class MealFoodChangedMessage : ValueChangedMessage<bool>
{
    public MealFoodChangedMessage() : base(true) { }
}
