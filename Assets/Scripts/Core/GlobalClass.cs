namespace Core
{
    public class GlobalClass : GlobalAccess
    {
        private static void AssignClass(GlobalAccess common)
        {
            Foundation.Globals.Add(common);
        }

        public virtual void OnDestroy()
        {
            if (Foundation.Globals.Contains(this))
                Foundation.Globals.Remove(this);
        }

        public virtual void Awake()
        {
            AssignClass(this);
        }
    }
}