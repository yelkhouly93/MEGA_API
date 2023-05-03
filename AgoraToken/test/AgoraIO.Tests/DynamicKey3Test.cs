using AgoraIO.Media;
using System;
using Xunit;

namespace AgoraIO.Tests
{
    public class DynamicKey3Test
    {
        String appID = "0fe4ac748cd24a9fb3ba4730ea201df9";
        String appCertificate = "3a313f24a1f243d19be7ac04ef3856af";
        String channel = "123456987";
        int ts = 1446455472;
        int r = 58964981;
        long uid = 2882341273L;
        int expiredTs = 1446455471;

        
        public void SetChannelName (string ChannelName)
        {
            channel = ChannelName;
        }

        [Fact]
        public void testGenerate() //throws Exception
        {


            String result = DynamicKey3.generate(appID, appCertificate, channel, ts, r, uid, expiredTs);

            String expected = "0037666966591a93ee5a3f712e22633f31f0cbc8f13970ca35de60c44645bbae8a215061b3314464554720383bbf528823412731446455471";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test2()
        {
            string expected = "7666966591a93ee5a3f712e22633f31f0cbc8f13";
            String version = "003";
            String unixTsStr = ("0000000000" + ts).Substring(ts.ToString().Length);
            String randomIntStr = ("00000000" + r.ToString("x4")).Substring(r.ToString("x4").Length);
            uid = uid & 0xFFFFFFFFL;
            String uidStr = ("0000000000" + uid.ToString()).Substring(uid.ToString().Length);
            String expiredTsStr = ("0000000000" + expiredTs.ToString()).Substring(expiredTs.ToString().Length);
            var result = DynamicKey3.generateSignature3(appID, appCertificate, channel, unixTsStr, randomIntStr, uidStr, expiredTsStr);
            Assert.Equal(expected, result);
        }
    }
}
