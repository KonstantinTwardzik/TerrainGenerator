namespace Topographer3D.Utilities
{
    internal static class Application
    {
        internal static float Apply(float oldValue, float applyValue, ApplicationMode CurrentApplicationMode)
        {
            float newValue = 0;
            switch (CurrentApplicationMode)
            {
                case ApplicationMode.Normal:
                    newValue = applyValue;
                    break;
                case ApplicationMode.Add:
                    newValue = oldValue + applyValue;
                    break;
                case ApplicationMode.Multiply:
                    newValue = oldValue * applyValue;
                    break;
                case ApplicationMode.Subtract:
                    newValue = oldValue - applyValue;
                    break;
            }
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 1)
            {
                newValue = 1;
            }
            return newValue;
        }
    }
}
