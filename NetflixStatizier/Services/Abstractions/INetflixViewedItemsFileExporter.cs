namespace NetflixStatizier.Services.Abstractions
{
    public interface INetflixViewedItemsFileExporter
    {
        byte[] GetFileContent();

        string GetFileName();

        string GetMimeType();
    }
}
