namespace Discord.Models;

public interface IMultipartParams
{
    IDictionary<string, object?> GetKeys();
    IDictionary<string, MultipartFile> GetFiles();
}