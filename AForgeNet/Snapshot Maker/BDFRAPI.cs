using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Baidu.Aip
{
    internal class FRDemo
    {
        static readonly string API_KEY = "n4i5GdULwGdBIi22SZ65QEZB";
        static readonly string SECRET_KEY = "sh2ZLIr4HiB5WwnFCiZmqVQ9gkaVzSet";
        public static void FaceMatch()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image1 = File.ReadAllBytes("图片文件路径");
            var image2 = File.ReadAllBytes("图片文件路径");
            var image3 = File.ReadAllBytes("图片文件路径");

            var images = new[] { image1, image2, image3 };

            // 人脸对比
            var result = client.FaceMatch(images);
        }

        public static JObject FaceDetect(string imagePath)
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image = File.ReadAllBytes(imagePath);
            var options = new Dictionary<string, object>
            {
                {"face_fields", "age,beauty,expression,faceshape,gender,glasses,landmark,race,qualities"}
            };
            var result = client.FaceDetect(image, options);
            return result;
        }

        public static JObject FaceRegister(string imagePath, string uid, string userInfo, string groupId = "groupId")
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image1 = File.ReadAllBytes(imagePath);

            var result = client.User.Register(image1, uid, userInfo, new[] { groupId });
            return result;
        }

        public static void FaceUpdate()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image1 = File.ReadAllBytes("图片文件路径");

            var result = client.User.Update(image1, "uid", "groupId", "new user info");
        }

        public static JObject FaceDelete(string uid, string groupId = "group1")
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.User.Delete(uid);
            result = client.User.Delete(uid, new[] { groupId });
            return result;
        }

        public static void FaceVerify()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image1 = File.ReadAllBytes("图片文件路径");

            var result = client.User.Verify(image1, "uid", new[] { "groupId" }, 1);
        }

        public static JObject FaceIdentify(string imagePath, string groupId = "groupId")
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var image1 = File.ReadAllBytes(imagePath);

            var result = client.User.Identify(image1, new[] { groupId }, 1, 1);
            return result;
        }

        public static void UserInfo()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.User.GetInfo("uid");
        }

        public static void GroupList()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.Group.GetAllGroups(0, 100);
        }

        public static void GroupUsers()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.Group.GetUsers("groupId", 0, 100);
        }

        public static void GroupAddUser()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.Group.AddUser(new[] { "toGroupId" }, "uid", "fromGroupId");
        }

        public static void GroupDeleteUser()
        {
            var client = new Face.Face(API_KEY, SECRET_KEY);
            var result = client.Group.DeleteUser(new[] { "groupId" }, "uid");
        }
    }
}