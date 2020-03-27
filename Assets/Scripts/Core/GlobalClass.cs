namespace Core
{
    public class GlobalClass : GlobalAccess
    {
        public virtual void Awake()
        {
            AssignClass(this);
        }

        public virtual void OnDestroy()
        {
            if (!Foundation.Globals.Contains(this)) return;
            
            Foundation.Globals.Remove(this);
        }

        private static void AssignClass(GlobalAccess common)
        {
            Foundation.Globals.Add(common);
        }
    }
}