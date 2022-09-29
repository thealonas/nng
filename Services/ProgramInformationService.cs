namespace nng.Services;

public class ProgramInformationService
{
    public readonly bool Debug;
    public readonly Version Version;

    public ProgramInformationService(Version version, bool debug)
    {
        Version = version;
        Debug = debug;
    }
}
