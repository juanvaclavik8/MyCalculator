namespace MyCalculator.DAL
{
    public interface IDataAccess
    {
        IList<string> GetComputationsHistory();

        void SaveComputation(string computation);
    }
}
