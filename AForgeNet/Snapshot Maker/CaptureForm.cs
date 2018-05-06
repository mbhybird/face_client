using System;
using System.Drawing;
using System.Windows.Forms;
///---添加名称空间
using AForge.Video.DirectShow;
using AForge.Video;
using System.IO;
using System.Drawing.Imaging;
using AForge.Vision.Motion;
using AFImageing = AForge.Imaging;

namespace Snapshot_Maker
{
    public partial class CaptureForm : Form
    {
        aidbEntities ctx = new aidbEntities();
        static readonly string IMAGE_PATH = Application.StartupPath + "\\temp.png";
        bool detected = false;
        int detectedCount = 0;
        public CaptureForm()
        {
            InitializeComponent();
        }
        ///---声明变量
        public FilterInfoCollection USE_Webcams = null;
        public VideoCaptureDevice cam = null;

        // motion detector
        MotionDetector detector = new MotionDetector(
            new TwoFramesDifferenceDetector(),
            new MotionAreaHighlighting());

        private float motionAlarmLevel = 0.015f;

        //---按钮被单击事件
        private void startBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (startBtn.Text == "开始")
                {
                    ///--
                    startBtn.Text = "停止";
                    if (rbH.Checked)
                    {
                        cam.VideoResolution = cam.VideoCapabilities[0];
                    }
                    else if (rbM.Checked)
                    {
                        cam.VideoResolution = cam.VideoCapabilities[1];
                    }
                    else {
                        cam.VideoResolution = cam.VideoCapabilities[2];
                    }
                    ///---启动摄像头
                    cam.Start();
                    timer1.Start();
                    if (detector != null)
                        detector.Reset();
                    detected = false;
                    detectedCount = 0;
                    btnRecord.Enabled = true;
                }
                else
                {
                    ///--设置按钮提示字样
                    startBtn.Text = "开始";
                    ///--停止摄像头捕获图像
                    cam.Stop();
                    timer1.Stop();
                    btnRecord.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //--窗口加载事件
        private void CaptureForm_Load(object sender, EventArgs e)
        {
            try
            {
                ///---实例化对象
                USE_Webcams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                ///---摄像头数量大于0
                if (USE_Webcams.Count > 0)
                {
                    ///---禁用按钮
                    startBtn.Enabled = true;
                    ///---实例化对象
                    cam = new VideoCaptureDevice(USE_Webcams[0].MonikerString);
                    ///---绑定事件
                    cam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                }
                else
                {
                    ///--没有摄像头
                    startBtn.Enabled = false;
                    ///---提示没有摄像头
                    MessageBox.Show("没有摄像头外设");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        ///------自定义函数
        private void Cam_NewFrame(object obj, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            float motionLevel = detector.ProcessFrame(eventArgs.Frame);
            if (motionLevel > motionAlarmLevel)
            {
                detected = true;
            }
            else
            {
                detected = false;
            }
        }


        ///---窗口关闭事件
        private void CaptureForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (cam != null)
                {
                    ///---关闭摄像头
                    if (cam.IsRunning)
                    {
                        cam.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(saveFileDialog.FileName).ToLower();
                ImageFormat format = ImageFormat.Jpeg;

                if (ext == ".bmp")
                {
                    format = ImageFormat.Bmp;
                }
                else if (ext == ".png")
                {
                    format = ImageFormat.Png;
                }

                try
                {
                    lock (this)
                    {
                        Bitmap image = (Bitmap)pictureBox1.Image;

                        image.Save(saveFileDialog.FileName, format);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed saving the snapshot.\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (rbFront.Checked)
            {
                pbFront.Image = pictureBox1.Image;
                pbFront.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image.Save(IMAGE_PATH);
            }
            else if (rbBack.Checked)
            {
                pbBack.Image = pictureBox1.Image;
                pbBack.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else if (rbLeft.Checked)
            {
                pbLeft.Image = pictureBox1.Image;
                pbLeft.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else {
                pbRight.Image = pictureBox1.Image;
                pbRight.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            var jsonObj = Baidu.Aip.FRDemo.FaceDetect(IMAGE_PATH);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
            txtResult.Text = json;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUID.Text.Trim())) return;
            if (string.IsNullOrEmpty(txtUserInfo.Text.Trim())) return;

            var jsonObj = Baidu.Aip.FRDemo.FaceRegister(IMAGE_PATH, txtUID.Text.Trim(), txtUserInfo.Text.Trim(), "befash_cn");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
            txtResult.Text = json;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            var jsonObj = Baidu.Aip.FRDemo.FaceIdentify(IMAGE_PATH, "befash_cn");
            if (jsonObj["result"] != null)
            {
                txtUID.Text = jsonObj["result"][0]["uid"].ToString();
            }
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
            txtResult.Text = json;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUID.Text.Trim()))
            {
                var jsonObj = Baidu.Aip.FRDemo.FaceDelete(txtUID.Text.Trim(), "befash_cn");
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj);
                txtResult.Text = json;
            }
        }

        private byte[] imageToByteArray(Image image)
        {
            if (image == null)
            {
                return null;
            }
            //MemoryStream ms = new MemoryStream();
            //image.Save(ms, ImageFormat.Png);
            //return ms.ToArray();

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (pbFront.Image == null)
            {
                MessageBox.Show("请先抓拍正面照片！");
                return;
            }

            var jsonObj = Baidu.Aip.FRDemo.FaceIdentify(IMAGE_PATH, "befash_cn");
            if (jsonObj["error_code"] == null)
            {
                var find = false;
                foreach (var score in jsonObj["result"][0]["scores"])
                {
                    if (Convert.ToInt16(score) > 80)
                    {
                        MessageBox.Show("老客户 - " + jsonObj["result"][0]["uid"].ToString());
                        var uid = jsonObj["result"][0]["uid"].ToString();
                        var groupId = jsonObj["result"][0]["group_id"].ToString();
                        var userInfo = jsonObj["result"][0]["user_info"].ToString();
                        //old customer

                        //analyst info
                        var infoObj = Baidu.Aip.FRDemo.FaceDetect(IMAGE_PATH);
                        var age = Convert.ToInt16(infoObj["result"][0]["age"]);
                        var gender = infoObj["result"][0]["gender"].ToString();
                        var glasses = Convert.ToInt16(infoObj["result"][0]["glasses"]);
                        var race = infoObj["result"][0]["race"].ToString();

                        //insert queue
                        ctx.queuetb.Add(new queuetb()
                        {
                            uid = uid,
                            groupid = groupId,
                            userinfo = userInfo,
                            type = "O",
                            front = imageToByteArray(pbFront.Image),
                            back = imageToByteArray(pbBack.Image),
                            left = imageToByteArray(pbLeft.Image),
                            right = imageToByteArray(pbRight.Image),
                            age = age,
                            gender = gender,
                            race = race,
                            glasses = glasses
                        });

                        ctx.SaveChanges();
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    MessageBox.Show("The guy is not identify - 新客户");
                    //new customer

                    //face register
                    var uid = "user_" + DateTime.Now.Ticks.ToString();
                    var groupId = "befash_cn";
                    var userInfo = "user create: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var regInfo = Baidu.Aip.FRDemo.FaceRegister(IMAGE_PATH, uid, userInfo, groupId);
                    //reg ok
                    if (regInfo["error_msg"] != null)
                    {
                        MessageBox.Show(regInfo["error_msg"].ToString());
                        return;
                    }

                    //analyst info
                    var infoObj = Baidu.Aip.FRDemo.FaceDetect(IMAGE_PATH);
                    var age = Convert.ToInt16(infoObj["result"][0]["age"]);
                    var gender = infoObj["result"][0]["gender"].ToString();
                    var glasses = Convert.ToInt16(infoObj["result"][0]["glasses"]);
                    var race = infoObj["result"][0]["race"].ToString();

                    //insert queue
                    ctx.queuetb.Add(new queuetb()
                    {
                        uid = uid,
                        groupid = groupId,
                        userinfo = userInfo,
                        type = "N",
                        front = imageToByteArray(pbFront.Image),
                        back = imageToByteArray(pbBack.Image),
                        left = imageToByteArray(pbLeft.Image),
                        right = imageToByteArray(pbRight.Image),
                        age = age,
                        gender = gender,
                        race = race,
                        glasses = glasses
                    });


                    ctx.SaveChanges();
                }
            }
            else
            {
                int code = Convert.ToInt32(jsonObj["error_code"]);
                //no user in group
                if (code == 216618)
                {
                    MessageBox.Show("The guy is not identify - 新客户");
                    //new customer

                    //face register
                    var uid = "user_" + DateTime.Now.Ticks.ToString();
                    var groupId = "befash_cn";
                    var userInfo = "user create: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var regInfo = Baidu.Aip.FRDemo.FaceRegister(IMAGE_PATH, uid, userInfo, groupId);
                    //reg ok
                    if (regInfo["error_msg"] != null)
                    {
                        MessageBox.Show(regInfo["error_msg"].ToString());
                        return;
                    }

                    //analyst info
                    var infoObj = Baidu.Aip.FRDemo.FaceDetect(IMAGE_PATH);
                    var age = Convert.ToInt16(infoObj["result"][0]["age"]);
                    var gender = infoObj["result"][0]["gender"].ToString();
                    var glasses = Convert.ToInt16(infoObj["result"][0]["glasses"]);
                    var race = infoObj["result"][0]["race"].ToString();

                    //insert queue
                    ctx.queuetb.Add(new queuetb()
                    {
                        uid = uid,
                        groupid = groupId,
                        userinfo = userInfo,
                        type = "N",
                        front = imageToByteArray(pbFront.Image),
                        back = imageToByteArray(pbBack.Image),
                        left = imageToByteArray(pbLeft.Image),
                        right = imageToByteArray(pbRight.Image),
                        age = age,
                        gender = gender,
                        race = race,
                        glasses = glasses
                    });


                    ctx.SaveChanges();
                }
                else {
                    MessageBox.Show(jsonObj["error_msg"].ToString() + "[" + code + "]");
                }
            }
            /*
              {
                "result_num": 1,
                "result": [
                    {
                        "location": {
                            "left": 117,
                            "top": 131,
                            "width": 172,
                            "height": 170
                        },
                        "face_probability": 1,
                        "rotation_angle": 2,
                        "yaw": -0.34859421849251,
                        "pitch": 2.3033397197723,
                        "roll": 1.9135693311691,
                        "landmark": [
                            {
                                "x": 161.74819946289,
                                "y": 163.30244445801
                            },
                            ...
                        ],
                        "landmark72": [
                            {
                                "x": 115.86531066895,
                                "y": 170.0546875
                            }，
                            ...
                        ],
                        "age": 29.298097610474,
                        "beauty": 55.128883361816,
                        "expression": 1,
                        "expression_probablity": 0.5543018579483,
                        "gender": "male",
                        "gender_probability": 0.99979132413864,
                        "glasses": 0,
                        "glasses_probability": 0.99999964237213,
                        "race": "yellow",
                        "race_probability": 0.99999976158142,
                        "qualities": {
                            "occlusion": {
                                "left_eye": 0,
                                "right_eye": 0,
                                "nose": 0,
                                "mouth": 0,
                                "left_cheek": 0.0064102564938366,
                                "right_cheek": 0.0057411273010075,
                                "chin": 0
                            },
                            "blur": 1.1886881756684e-10,
                            "illumination": 141,
                            "completeness": 1,
                            "type": {
                                "human": 0.99935841560364,
                                "cartoon": 0.00064159056637436
                            }
                        }
                    }
                ],
                "log_id": 2493878179101621
             }
             {
             "error_code":216402,
             "error_msg":"face not found",
             "log_id":2550761986122018
             }
             {
             "result":[{
             "uid":"xiaoyoutiao",
             "scores":[24.431762695312],
             "group_id":"befash_cn",
             "user_info":"app developer"
             }],"result_num":1,"log_id":2535345581122018
             }
             */
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (detected)
            {
                label5.ForeColor = Color.Red;
                label5.Text = "Motion Detected";
                detectedCount++;
            }
            else
            {
                label5.ForeColor = Color.Green;
                label5.Text = "No Motion Detected";

                if (detectedCount > 0)
                {
                    detectedCount = 0;
                    timer1.Stop();
                    btnRecord_Click(sender, e);
                    btnOK_Click(sender, e);
                    timer1.Start();
                }
            }
        }
    }
}