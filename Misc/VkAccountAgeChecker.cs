using System.Xml;
using nng.Exceptions;

namespace nng.Misc;

public static class VkAccountAgeChecker
{
    public static DateTime GetAccountAge(long vkId)
    {
        XmlReader reader;
        try
        {
            reader = new XmlTextReader(Foaf.GetAccountUrl((int) vkId));
        }
        catch (Exception)
        {
            throw new VkFrameworkMethodException(nameof(GetAccountAge),
                $"Документ foaf'а на ID {vkId} пустой");
        }

        try
        {
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;
                if (reader.Name != "ya:created") continue;
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name != "dc:date") continue;
                    return DateTime.Parse(reader.Value);
                }
            }
        }
        catch (Exception e)
        {
            throw new VkFrameworkMethodException(nameof(GetAccountAge), $"{e.GetType()}: {e.Message}");
        }

        throw new VkFrameworkMethodException(nameof(GetAccountAge), $"В foaf'е отсутсвует ya:created на ID {vkId}");
    }
}
