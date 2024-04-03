using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.CvEnum;


namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        #region Variables
        
        string connectstring = @"Data Source=DESKTOP-4B7JTQE\THANHSON;Initial Catalog=SQL_login;Integrated Security=True";
        SqlConnection connect;
        SqlCommand cmd;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        

        MJPEGStream stream;
        Mat frame = new Mat();
        private bool facesDetectionEnabled = false;
        private CascadeClassifier faceCascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        List<Image<Gray, Byte>> TrainedFaces = new List<Image<Gray, Byte>>();
        List<int> PersonLabel = new List<int>();
        bool EnableSaveImage = false;
        private static bool istrained = false;
        LBPHFaceRecognizer recognizer;
        List<String> PersonsNames = new List<string>();
        #endregion
        public Form1()
        {
            InitializeComponent();
            
        }

        private void btn_get_Click(object sender, EventArgs e) 
        {
            stream = new MJPEGStream(txt_addlink.Text);
            stream.NewFrame += new NewFrameEventHandler(video_NewFrame);
            stream.Start();
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
          
            if (eventArgs.Frame == null) return;
            //Mat frame = new Image<Bgr, byte>((Bitmap)eventArgs.Frame).Mat;
            Image<Bgr, Byte> emguFrame = new Image<Bgr, Byte>((Bitmap)eventArgs.Frame);
            if (facesDetectionEnabled)
            {
                Mat grayImage = new Mat();
                CvInvoke.CvtColor(emguFrame, grayImage, ColorConversion.Bgr2Gray);
                CvInvoke.EqualizeHist(grayImage, grayImage);
                Rectangle[] faces = faceCascadeClassifier.DetectMultiScale(grayImage, 1.15, 4, Size.Empty, Size.Empty);
                if(faces.Length > 0)
                {
                    foreach(var face in faces)
                    {
                        //CvInvoke.Rectangle(emguFrame, face, new Bgr(Color.Red).MCvScalar,3);

                        Image<Bgr, Byte> resultImage = emguFrame.Copy(face);
                       
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBox2.Image = resultImage.Bitmap;

                        if (EnableSaveImage)
                        {
                            // Tạo thư mục gốc nếu nó chưa tồn tại
                            string path = Directory.GetCurrentDirectory() + @"\TrainedImage";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);

                            // Tạo thư mục cho người này nếu nó chưa tồn tại
                            string personFolder = Path.Combine(path, txt_name.Text);
                            if (!Directory.Exists(personFolder))
                                Directory.CreateDirectory(personFolder);

                            Task.Factory.StartNew(() => {
                                for (int i = 0; i < 5; i++)
                                {
                                    Image<Gray, Byte> gray = resultImage.Convert<Gray, Byte>();
                                    gray.Resize(200, 200, Inter.Cubic).Save(Path.Combine(personFolder, $"{txt_name.Text}_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}.jpg"));
                                    Thread.Sleep(1000);
                                }
                            });
                        }
                        EnableSaveImage = false;
                        if (btn_addpsn.InvokeRequired)
                        {
                            btn_addpsn.Invoke(new ThreadStart(delegate
                            {
                                btn_addpsn.Enabled = true;
                            }));
                        }

                        if (istrained)
                        {
                            Image<Gray, Byte> grayFaceResult = resultImage.Convert<Gray, Byte>().Resize(200, 200, Inter.Cubic);
                            CvInvoke.EqualizeHist(grayFaceResult, grayFaceResult);
                            var result = recognizer.Predict(grayFaceResult);
                           
                            
                            Debug.WriteLine(result.Label + ". " + result.Distance);
                            if (result.Label >= 0 && result.Label < TrainedFaces.Count && result.Distance <50)
                            {
                                SendUDPMessage(true);
                                //cmd.CommandText = "insert into AccessLog(TenUser, TimeAccess, AccData) values(N'" + PersonsNames[result.Label] + "', '" + DateTime.Now.ToString() + "', '" + "Open by FaceID" + "')";
                                //cmd.ExecuteNonQuery();
                                CvInvoke.PutText(emguFrame, PersonsNames[result.Label], new Point(face.X -2, face.Y -2),
                                        FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                CvInvoke.Rectangle(emguFrame, face, new Bgr(Color.Green).MCvScalar, 2);
                            }
                            else
                            {
                                CvInvoke.PutText(emguFrame, "Unknown", new Point(face.X -2, face.Y -2), 
                                    FontFace.HersheyComplex, 1.0, new Bgr(Color.Orange).MCvScalar);
                                CvInvoke.Rectangle(emguFrame, face, new Bgr(Color.Green).MCvScalar, 2);
                            }
                        }
                    }
                }
            }

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = emguFrame.Bitmap;
            
        }
        private bool TrainImageFromDir()
        {
            
            TrainedFaces.Clear();
            PersonLabel.Clear();
            PersonsNames.Clear();
            try
            {
                string path = Directory.GetCurrentDirectory() + @"\TrainedImage";
                string[] files = Directory.GetFiles(path, "*.jpg", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    Image<Gray, Byte> trainedImage = new Image<Gray, Byte>(file).Resize(200, 200, Inter.Cubic);
                    CvInvoke.EqualizeHist(trainedImage, trainedImage);
                    TrainedFaces.Add(trainedImage);
                    string name = Path.GetFileNameWithoutExtension(file).Split('_')[0];
                    if (!PersonsNames.Contains(name))
                    {
                        PersonsNames.Add(name);
                    }
                    PersonLabel.Add(PersonsNames.IndexOf(name));
                    Debug.WriteLine(PersonLabel.Last() + ". " + name);
                }
                for(int i=0; i< TrainedFaces.Count(); i++)
                {
                    recognizer = new LBPHFaceRecognizer(1, 8, 8,8, 200);
                    recognizer.Train(TrainedFaces.ToArray(), PersonLabel.ToArray());
                }
                ;

                istrained = true;
                return true;
            }
            catch (Exception ex)
            {
                istrained = false;
                MessageBox.Show("Error in Train Images: " + ex.Message);
                return false;
            }
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            
            stream.Stop();
        }

        private void btn_addpsn_Click(object sender, EventArgs e)
        {
           
            btn_addpsn.Enabled = false;
            EnableSaveImage = true;
        }

        private void btn_detect_face_Click(object sender, EventArgs e)
        {
            facesDetectionEnabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thdUDPServer = new Thread(new ThreadStart(serverThread));
            thdUDPServer.Start();
            connect = new SqlConnection(connectstring);
            connect.Open();
        }
        
        public void serverThread()
        {
            UdpClient udpClient = new UdpClient(8080);
            while (true)
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8080);
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes,0, receiveBytes.Length);
                Debug.WriteLine(returnData);
                string[] parts = returnData.Split('/');
                if(parts[0]=="Master")
                {
                    cmd = connect.CreateCommand();
                    cmd.CommandText = "insert into AccessLog(TenUser, TimeAccess, AccData) values('" + parts[0] + "', '" + DateTime.Now.ToString() + "', '" + parts[1] + "')";
                    cmd.ExecuteNonQuery();
                    
                }
                else 
                {
                    string RFID = parts[0];
                    
                    Debug.WriteLine(RFID);
                    string query = "select u.TenUser " +
                        "from DataUser as u "+
                        "where u.RFID= @rfid";
                    cmd = connect.CreateCommand();
                    cmd.CommandText= query;
                    cmd.Parameters.AddWithValue("@rfid", RFID);
                    string TenUser = "";
                    string ThemThe = "";
                    try
                    {
                        
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            TenUser = reader["TenUser"].ToString();
                        }
                        reader.Close();
                        Debug.WriteLine(TenUser);
                        cmd = connect.CreateCommand();
                        if (parts[1]== "Delete UID")
                        {
                            cmd.CommandText = "delete from AccessLog where TenUser=N'"+TenUser+"'"; 
                            cmd.ExecuteNonQuery(); 
                            
                            cmd.CommandText = "delete from DataUser where TenUser=N'" + TenUser + "'";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("xóa thẻ thành công");
                        }
                        else if(parts[1]== "Add an UID")
                        {
                            ThemThe = txt_themThe.Text;
                            cmd.CommandText = "insert into DataUser(TenUser, RFID) values(N'" + ThemThe + "', '" + RFID + "' ) ";
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Thêm thành công");
                            txt_themThe.Text = "";
                        }
                        else
                        {
                            cmd.CommandText = "insert into AccessLog(TenUser, TimeAccess, AccData) values(N'" + TenUser + "', '" + DateTime.Now.ToString() + "', '" + parts[1] + "')";
                            cmd.ExecuteNonQuery(); 
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }  
                }
            }
        }

        
        
         private void SendUDPMessage(bool value)
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Connect("192.168.108.107", 8080);
                Byte[] senddata = BitConverter.GetBytes(value);
                udpClient.Send(senddata, senddata.Length);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnTrain_Click(object sender, EventArgs e)
        {
            TrainImageFromDir();
        }

        private void btn_log_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
