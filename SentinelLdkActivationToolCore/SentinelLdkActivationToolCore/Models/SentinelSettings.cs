using System.Collections.Generic;

namespace SentinelLdkActivationToolCore.Models
{
    public class SentinelSettings
    {
        public static bool ignoreSslCertStatus { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:IgnoreSslCertStatus"] == "true" ||
                                                            ConfigurationManager.AppSetting["EmsServerSettings:IgnoreSslCertStatus"] == "True" ||
                                                            ConfigurationManager.AppSetting["EmsServerSettings:IgnoreSslCertStatus"] == "1" ? true : false;

        public static string emsProtocol { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:EmsProtocol"];

        public static string emsAddress { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:EmsAddress"];

        public static string emsPort { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:EmsPort"];

        public static string emsBaseDir { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:EmsBaseDir"];

        public static string webServiceVersion { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:EmsWebServiceVersion"]; 

        public static string batchCode { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:BatchCode"];

        public static string vendorId { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:VendorId"];

        public static string vendorCode { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:VendorCode"];

        public static string vendorLogin { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:VendorAuthData:Login"];

        public static string vendorPassword { get; set; } = ConfigurationManager.AppSetting["EmsServerSettings:VendorAuthData:Password"];

        public static string testProductKey { get; set; } = ConfigurationManager.AppSetting["TestData:TestProductKey"];

        public static Dictionary<string, string> actionsList { get; set; } = new Dictionary<string, string> {
            { "login", @"ws/login.ws" },                                            // done
            { "loginpk", @"ws/loginByProductKey.ws" },                              // done
            { "logout", @"ws/logout.ws" },                                          // done
            { "getinfo", @"ws/productKey/{PLACEHOLDER}.ws" },                       // done
            { "getact", @"ws/productKey/{PLACEHOLDER}/activation.ws" },             // done
            { "dc2v", @"ws/c2v/decodeC2V.ws" },                                     // not yet done
            { "getfpu", @"ws/activation/target.ws" },                               // done
            { "ukey", @"ws/target.ws" },                                            // not yet done
            { "getlic", @"ws/activation/{PLACEHOLDER}.ws" }                         // done
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
