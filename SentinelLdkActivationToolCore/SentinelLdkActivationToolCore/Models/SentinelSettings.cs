using System.Collections.Generic;

namespace SentinelLdkActivationToolCore.Models
{
    public class SentinelSettings
    {
        public static string emsProtocol { get; set; } = "http";

        public static string emsAddress { get; set; } = @"rsk-inc.site"; // @"185.20.226.150";

        public static string emsPort { get; set; } = @"8080";

        public static string emsBaseDir { get; set; } = @"ems";

        public static string webServiceVersion { get; set; } = @"v80";

        public static string batchCode { get; set; } = @"DEMOMA";

        public static string vendorId { get; set; } = @"37515";

        public static string vendorCode { get; set; } = "AzIceaqfA1hX5wS+M8cGnYh5ceevUnOZIzJBbXFD6dgf3tBkb9cvUF/Tkd/iKu2fsg9wAysYKw7RMAsV" +
                                                        "vIp4KcXle/v1RaXrLVnNBJ2H2DmrbUMOZbQUFXe698qmJsqNpLXRA367xpZ54i8kC5DTXwDhfxWTOZrB" +
                                                        "rh5sRKHcoVLumztIQjgWh37AzmSd1bLOfUGI0xjAL9zJWO3fRaeB0NS2KlmoKaVT5Y04zZEc06waU2r6" +
                                                        "AU2Dc4uipJqJmObqKM+tfNKAS0rZr5IudRiC7pUwnmtaHRe5fgSI8M7yvypvm+13Wm4Gwd4VnYiZvSxf" +
                                                        "8ImN3ZOG9wEzfyMIlH2+rKPUVHI+igsqla0Wd9m7ZUR9vFotj1uYV0OzG7hX0+huN2E/IdgLDjbiapj1" +
                                                        "e2fKHrMmGFaIvI6xzzJIQJF9GiRZ7+0jNFLKSyzX/K3JAyFrIPObfwM+y+zAgE1sWcZ1YnuBhICyRHBh" +
                                                        "aJDKIZL8MywrEfB2yF+R3k9wFG1oN48gSLyfrfEKuB/qgNp+BeTruWUk0AwRE9XVMUuRbjpxa4YA67SK" +
                                                        "unFEgFGgUfHBeHJTivvUl0u4Dki1UKAT973P+nXy2O0u239If/kRpNUVhMg8kpk7s8i6Arp7l/705/bL" +
                                                        "Cx4kN5hHHSXIqkiG9tHdeNV8VYo5+72hgaCx3/uVoVLmtvxbOIvo120uTJbuLVTvT8KtsOlb3DxwUrwL" +
                                                        "zaEMoAQAFk6Q9bNipHxfkRQER4kR7IYTMzSoW5mxh3H9O8Ge5BqVeYMEW36q9wnOYfxOLNw6yQMf8f9s" +
                                                        "JN4KhZty02xm707S7VEfJJ1KNq7b5pP/3RjE0IKtB2gE6vAPRvRLzEohu0m7q1aUp8wAvSiqjZy7FLaT" +
                                                        "tLEApXYvLvz6PEJdj4TegCZugj7c8bIOEqLXmloZ6EgVnjQ7/ttys7VFITB3mazzFiyQuKf4J6+b/a/Y";

        public static Dictionary<string, string> actionsList { get; set; } = new Dictionary<string, string> {
            { "login", @"ws/login.ws" },                                    // not yet done
            { "loginpk", @"ws/loginByProductKey.ws" },                            // done
            { "logout", @"ws/logout.ws" },                                          // not yet done
            { "getinfo", @"ws/productKey/{PLACEHOLDER}.ws" },                     // done
            { "getact", @"ws/productKey/{PLACEHOLDER}/activation.ws" },      // done
            { "dc2v", @"ws/c2v/decodeC2V.ws" },                                // not yet done
            { "fpu", @"ws/activation/target.ws" },                  // not yet done
            { "ukey", @"ws/target.ws" },                             // not yet done
            { "getlic", @"ws/activation/{PLACEHOLDER}.ws" }                // done
        };

        // ======= XML =======

        public static string pkInfoXmlString { get; set; } = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                                                                "{PLACEHOLDER}";

        public static string activationXmlString { get; set; } = "<activation>" +
                                                               "<activationInput>" +
                                                                  "<activationAttribute>" +
                                                                     "<attributeValue>" +
                                                                        "<![CDATA[{PLACEHOLDER}]]>" +
                                                                        "</attributeValue>" +
                                                                     "<attributeName>C2V</attributeName>" +
                                                                  "</activationAttribute>" +
                                                                  "<comments></comments>" +
                                                               "</activationInput>" +
                                                            "</activation>";

        public static string authXmlString { get; set; } = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                                "<authenticationDetail>" +
                                                                      "<userName>{PLACEHOLDER_LOGIN}</userName>" +
                                                                      "<password>{PLACEHOLDER_PASSWORD}</password>" +
                                                                "</authenticationDetail>";

    }
}
