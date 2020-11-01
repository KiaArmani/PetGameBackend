using System;

namespace PetGameBackend.XTests
{
    public static class Setup
    {
        public static void SetupEnvironment()
        {
            Environment.SetEnvironmentVariable("MT_MONGODB_CONNECTION",
                "REDACTED");
            Environment.SetEnvironmentVariable("MT_MONGODB_DATABASE", "tamagotchi");
            Environment.SetEnvironmentVariable("MT_MONGODB_COLLECTION", "data");
        }
    }
}