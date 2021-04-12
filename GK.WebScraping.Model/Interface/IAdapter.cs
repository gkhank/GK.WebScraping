namespace GK.WebScraping.Model.Interface
{
    public interface IAdapter<ResponseType>
    {
        public GenericResponse Convert(IStore store, ResponseType response, SearchOptions options);
    }
}
