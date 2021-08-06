namespace GarryDB.Specs
{
    internal abstract class TestDataBuilder<TSubject>
    {
        public TSubject Build()
        {
            OnPreBuild();
            TSubject subject = OnBuild();
            OnPostBuild(subject);

            return subject;
        }

        protected virtual void OnPreBuild()
        {
        }

        protected abstract TSubject OnBuild();

        protected virtual void OnPostBuild(TSubject subject)
        {
        }
    }
}
