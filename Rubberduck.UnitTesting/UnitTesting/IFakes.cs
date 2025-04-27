namespace Rubberduck.UnitTesting
{
    public interface IFakes
    {
        void StartTest();
        void StopTest();

        void StartHeadlessTest();
    }

    public interface IFakesFactory
    {
        IFakes Create();
    }
}
