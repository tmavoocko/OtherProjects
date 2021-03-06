﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using System.Net;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
//using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Drawing.Printing;

namespace ExRaChe
{
    public partial class ExchSniff : Form
    {
        //Manager
        public bool ShwNws = false;
        private static Font TitleFont = new Font("Litograph", 10, FontStyle.Bold);
        private static Font ContentFont = new Font("Litograph", 10, FontStyle.Regular);
        private static Font NfoFont = new Font("Litograph", 8, FontStyle.Regular);
        public float FontSize
        {
            get { return TitleFont.SizeInPoints; }
            set
            {
                float input = value;
                if (input < 10) input = 10;
                if (input > 40) input = 40;
                TitleFont = new Font(TitleFont.FontFamily, input, TitleFont.Style);
                //TitleLabel.Font = TitleFont;
                ContentFont = new Font(ContentFont.FontFamily, input, ContentFont.Style);
                NfoFont = new Font(ContentFont.FontFamily, input, ContentFont.Style);
                Refresh();
                //ContentLabel.Font = ContentFont;
            }
        }
        private int ShwOneCrrncsPosition=0, LngSetting = 0;
        private string BttLngXm = "", TtlXm = "", NwCrrTxtXm = "", LblStrtDXm0 = "", LblStrtDXm1 = "", LblStrtDXm2 = "", LblStrtDXm3 = "", HntLstDataRefresh="", HntCrr0 = "", HntCrr1 = "", BttStrtDXm = "", BttEndDXm = "", XmMnth1="", XmMnth3 = "", XmMnth6 = "", XmMnth12 = "", XmMnthAll="";
        public Color ClrdBck = Color.FromArgb(132, 45, 37);
        public Color ClrdFrnt = Color.FromArgb(232, 145, 137);

        public Color ClrInver;
        public Control LstCntrl = new Control();
        private int DsgnModifierWdth = 2;
        public int SiModifier = 1;//Manager

        //IntroScr
        public int TmrCnvertToInt = 0;
        public int TmrCnvertToIntEnd = 0;
        public int ClctIntrval = 0;
        public DateTime TmerStart = new DateTime();
        public DateTime TmerEnd = new DateTime();
        public DateTime TmOfClick = new DateTime();
        List<string> IntroStrngs = new List<string>();
        private Point mouseDownLoc;
        public Panel DrwCore = new Panel();//IntroScr

        //Hrzntl Base DSGN object declaration
        public Point SpwLc = new Point();
        public Timer Tckr1 = new Timer();
        public Font MFnt = new Font("Litograph", 15, FontStyle.Bold);
        public Font mFntCh = new Font("Litograph", 10, FontStyle.Regular);
        public Rectangle DsgRct = new Rectangle();
        public Panel VrtLft, HrzLftTop, HrzLftDwn, VrtRght, HrzRghtTop, HrzRghtDwn;
        public Panel Hrzntl = new Panel();
        public Panel Prgrm = new Panel();
        public Color ClrBasePenelHolder = Color.FromArgb(255, Color.WhiteSmoke);//Hrzntl Base DSGN object declaration

        //Hrzntl everithing else DSGN-object declaration -new
        public MainStatus Sttus = new MainStatus();
        private Label Ttle = new Label();
        private Label ttle;
        private Label NCrr = new Label();
        private Label UrlImg = new Label();
        ////private GrphPanel shwGraph;
        private PictureBox shwPicture = new PictureBox() { Dock = DockStyle.Fill, MinimumSize = new Size(30, 16), /*MaximumSize = new Size(900, 600), */SizeMode = PictureBoxSizeMode.StretchImage, Visible = true };
        private Label shwSummary;
        private Label LblSrSDate;
        private Label LblSrEDate;
        private Panel allwaysShown = new Panel();
        private Panel btnHldr = new Panel();
        private PictureBox icn = new PictureBox();
        private Label Icn = new Label();
        private Label LstDataRefresh = new Label();
        private bool Fake = false;
        private Size MSzz = new Size();
        public Panel smtimsShown = new Panel();//Hrzntl everithing elseDSGN
        //Dsgn nwFrm
        private Form nwFrm;//Dsgn nwFrm

        //Graph object declaration
        private bool tst = true;
        private List<PriceData> PriceList = new List<PriceData>();// Store the data.
        private List<PriceData> nvrtd = new List<PriceData>();
        private List<PriceData> prcDateTemporary = new List<PriceData>();
        private Matrix WtoDMatrix, DtoWMatrix;// The coordinate mappings.
        private Label txtStartDate = new Label() { TextAlign = ContentAlignment.MiddleLeft, Font = ContentFont, ForeColor = Color.Black, BackColor = Color.Yellow, Dock = DockStyle.Left };
        private Label txtEndDate = new Label() { TextAlign = ContentAlignment.MiddleRight, Font = ContentFont, ForeColor = Color.Black, BackColor = Color.Yellow, Dock = DockStyle.Right };
        private ToolTip tipData = new ToolTip();//Graph object declaration

        //User DateTime
        private DateTime SrEnd_date;
        private DateTime SrStart_date;//User DateTime

        //SniffIt() object declaration
        private Dictionary<string, decimal> CrrncsRates = new Dictionary<string, decimal>();
        private Dictionary<DateTime, decimal> CrrncyHstry = new Dictionary<DateTime, decimal>();//NEW -SniffIt() object declaration
        private List<XmlNode> NodesAll = new List<XmlNode>();//SniffIt() object declaration

        

        private Timer tcker = new Timer() { Interval = 100 };
        private int tickCounter = 0;
        private bool updateWatcher = false;
        public ExchSniff()
        {

            {//Manager stuff
                {
                    

                    //FormBorderStyle = FormBorderStyle.None;
                    //WindowState = FormWindowState.Maximized;
                    DoubleBuffered = true;
                    //AutoScroll = true;           
                    //InitializeComponent();
                    //Text = "TxtSpltr";
                    Text = "Exchange rate checker - D.Konicek - zadani pate";
                    MouseDown += (sender, e) =>
                    {
                        mouseDownLoc = e.Location;

                    };

                    MouseUp += (sender, e) =>
                    {

                    };
                    MouseWheel += (sender, e) =>
                    {
                        //int oldZoom = 100;
                        //int newZoom = e.Delta * SystemInformation.MouseWheelScrollLines / 30 + ZoomPerc;
                        //ZoomPerc += e.Delta * SystemInformation.MouseWheelScrollLines / 120;
                        //if (newZoom < minZoom) newZoom = minZoom;
                        //if (newZoom > maxZoom) newZoom = maxZoom;
                        //foreach (Control p in Controls)
                        //{
                        //    p.Location = new Point(p.Location.X * newZoom / oldZoom, p.Location.Y * newZoom / oldZoom);
                        //    p.Size = new Size(p.Size.Width * newZoom / oldZoom, p.Size.Height * newZoom / oldZoom);
                        //    //MessageBox.Show(p.Location.ToString() +Environment.NewLine + p.Size.ToString());
                        //}
                    };
                    MouseHover += (sender, e) =>
                    {
                        ToolTip hint = new ToolTip();
                        hint.SetToolTip(this, Text);
                        foreach (Control c in Controls)
                        {
                            c.MouseHover += (senderf, f) =>
                            {
                                ToolTip hint2 = new ToolTip();
                                hint2.SetToolTip(c, c.ToString());
                            };
                            if (c.Controls.Count > 0)
                            {
                                foreach (Control Cc in Controls)
                                {

                                    Cc.MouseHover += (senderf, f) =>
                                    {
                                        ToolTip hint2 = new ToolTip();
                                        hint2.SetToolTip(Cc, Cc.ToString());
                                    };
                                    if (Cc.Controls.Count > 0)
                                    {
                                        foreach (Control Ccc in Controls)
                                        {
                                            Ccc.MouseHover += (senderf, f) =>
                                            {
                                                ToolTip hint2 = new ToolTip();
                                                hint2.SetToolTip(Ccc, Ccc.ToString());
                                            };
                                        }
                                    }
                                }

                            }
                        }

                    };
                    MouseLeave += (sender, e) =>
                    {

                        {
                            ////No mdFrameLoc - because it is on Form1                        
                            //Point parentLoc = new Point(0, 0);
                            //try { Cnst.jstTry.Dispose(); } catch (Exception) { }
                            //try { Cnst.justTwo.Dispose(); } catch (Exception) { }
                            //try { Cnst.justOne.Dispose(); } catch (Exception) { }
                            //BackColor = ClrdBck;
                            //Enabled = true;
                            ////Cnst.jstTry = new MTltip(this, this, "Pokus!!!!", 70);
                            ////Cnst.jstTry.BringToFront();
                        }
                    };
                    KeyUp += (sender, e) =>
                    {
                        //if (e.KeyData == Keys.Enter)
                        //{
                        //    //No mdFrameLoc - because it is on Form1                        
                        //    Point parentLoc = new Point(0, 0);
                        //    try { Cnst.justTwo.Dispose(); } catch (Exception) { }
                        //    try { Cnst.justOne.Dispose(); } catch (Exception) { }
                        //    Cnst.jstTry = new MTltip(parentLoc, this);
                        //    Cnst.jstTry.BringToFront();
                        //}
                    };
                    FormClosing += (sender, e) =>
                    {
                        {//Xml create & save

                            XmlDocument doc = new XmlDocument();
                            XmlElement prgr = (XmlElement)doc.AppendChild(doc.CreateElement("prgrm"));

                            prgr.SetAttribute("ExchangeRateChecker", this.Text + DateTime.Now.ToString());
                            {//ShwOneCrrncsPosition-(prgrm InnerNode) 
                                XmlElement ShwOneCrrncsPositionXml = (XmlElement)prgr.AppendChild(doc.CreateElement("ShwOneCrrncsPositionXml"));
                                ShwOneCrrncsPositionXml.InnerText = "SOCPXmlInt";
                                //ShwOneCrrncsPositionXml.Value = ShwOneCrrncsPosition.ToString();
                                ShwOneCrrncsPositionXml.SetAttribute("ShwOneCrrncsPositionXmlInt", ShwOneCrrncsPosition.ToString());
                            }//ShwOneCrrncsPosition-(prgrm InnerNode)
                            {//LngSetting-(prgrm InnerNode) 
                                XmlElement LngSettingXml = (XmlElement)prgr.AppendChild(doc.CreateElement("LngSettingXml"));
                                LngSettingXml.InnerText = "LNGXmlInt";
                                LngSettingXml.SetAttribute("LngSettingXmlInt", LngSetting.ToString());
                            }//LngSetting-(prgrm InnerNode)
                            {//cBlok-InnerNode 
                             //XmlElement cBlock = (XmlElement)prgr.AppendChild(doc.CreateElement("CBlok"));
                             //cBlock.InnerText = "CBlock-InnerNode";
                             ////cBlock.SetAttribute("cBlock", Cnst.C.UsVl.Text);
                            }//cBlok-InnerNode
                             //MessageBox.Show(doc.OuterXml);

                            // Save the document to a file. White space is
                            // preserved (no white space).
                            doc.PreserveWhitespace = true;
                            doc.Save("ExchangeRateChecker.xml");
                        }//Xml create & save
                    };
                    {//Language.xml create & save
                        {//Language.xml create & save

                            XmlDocument docc = new XmlDocument();
                            XmlElement Lnguage = (XmlElement)docc.AppendChild(docc.CreateElement("Lnguage"));

                            Lnguage.SetAttribute("ExchangeRateChecker", this.Text + DateTime.Now.ToString() + " //Created by programator: H.Vondrašová - Czech republic, Prague");

                            //HntLstDataRefresh
                            {//HntLstDataRefresh-(Lnguage InnerNode) 
                                XmlElement HntLstDataRefresh = (XmlElement)Lnguage.AppendChild(docc.CreateElement("HntLstDataRefresh"));
                                HntLstDataRefresh.SetAttribute("czVersion", "Čas poslední obnovy dat");
                                HntLstDataRefresh.SetAttribute("enVersion", "Last time of refresh");
                            }//HntLstDataRefresh-(Lnguage InnerNode)
                            {//Currency-(Lnguage InnerNode) 
                                XmlElement Currency = (XmlElement)Lnguage.AppendChild(docc.CreateElement("Currency"));
                                Currency.SetAttribute("czVersion", "Měna:");
                                Currency.SetAttribute("enVersion", "Currency:");
                            }//Currency-(Lnguage InnerNode)
                            {//HntCrr0-(Lnguage InnerNode) 
                                XmlElement HntCrr0 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("HntCrr0"));
                                HntCrr0.SetAttribute("czVersion", "Ukázat více");
                                HntCrr0.SetAttribute("enVersion", "View more.");
                            }//HntCrr0-(Lnguage InnerNode)
                            {//HntCrr1-(Lnguage InnerNode) 
                                XmlElement HntCrr1 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("HntCrr1"));
                                HntCrr1.SetAttribute("czVersion", "Ukázat méně");
                                HntCrr1.SetAttribute("enVersion", "View less.");
                            }//HntCrr1-(Lnguage InnerNode)
                            {//NwCrrTxt-(Lnguage InnerNode) 
                                XmlElement NwCrrTxt = (XmlElement)Lnguage.AppendChild(docc.CreateElement("NwCrrTxt"));
                                NwCrrTxt.SetAttribute("czVersion", "koupí 1");
                                NwCrrTxt.SetAttribute("enVersion", "buys 1");
                            }//NwCrrTxt-(Lnguage InnerNode)

                            //
                            {//BttLngXm-(Lnguage InnerNode) 
                                XmlElement BttLngXm = (XmlElement)Lnguage.AppendChild(docc.CreateElement("BttLngXm"));
                                BttLngXm.SetAttribute("czVersion", "Jazyk");
                                BttLngXm.SetAttribute("enVersion", "Language");
                            }//BttLngXm-(Lnguage InnerNode)

                            {//BttStrtDXm-(Lnguage InnerNode) 
                                XmlElement BttStrtDXm = (XmlElement)Lnguage.AppendChild(docc.CreateElement("BttStrtDXm"));
                                BttStrtDXm.SetAttribute("czVersion", "Začátek");
                                BttStrtDXm.SetAttribute("enVersion", "Start");
                            }//BttStrtDXm-(Lnguage InnerNode)
                            {//BttEndDXm-(Lnguage InnerNode) 
                                XmlElement BttEndDXm = (XmlElement)Lnguage.AppendChild(docc.CreateElement("BttEndDXm"));
                                BttEndDXm.SetAttribute("czVersion", "Konec");
                                BttEndDXm.SetAttribute("enVersion", "End");
                            }//BttEndDXm-(Lnguage InnerNode)

                            {//LblStrtDXm0-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement LblStrtDXm0 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("LblStrtDXm0"));
                                LblStrtDXm0.SetAttribute("czVersion", "Požadovaná data nejsou k dispozici");
                                LblStrtDXm0.SetAttribute("enVersion", "Requested data are not available");
                            }//LblStrtDXm0-(Lnguage InnerNode)
                            {//LblStrtDXm1-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement LblStrtDXm1 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("LblStrtDXm1"));
                                LblStrtDXm1.SetAttribute("czVersion", "Vyberte, prosím, jiné datum zahájení:");
                                LblStrtDXm1.SetAttribute("enVersion", "Please select different start date then: ");
                            }//LblStrtDXm1-(Lnguage InnerNode)
                            {//LblStrtDXm2-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement LblStrtDXm2 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("LblStrtDXm2"));
                                LblStrtDXm2.SetAttribute("czVersion", "Vybrané počáteční datum: ");
                                LblStrtDXm2.SetAttribute("enVersion", "Selected start date: ");
                            }//LblStrtDXm2-(Lnguage InnerNode)
                            {//LblStrtDXm3-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement LblStrtDXm3 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("LblStrtDXm3"));
                                LblStrtDXm3.SetAttribute("czVersion", "Chybná volba, prosím vyberte znovu.");
                                LblStrtDXm3.SetAttribute("enVersion", "Wrong choice, please reselect.");
                            }//LblStrtDXm3-(Lnguage InnerNode)
                            {//LblStrtDXm4-(Lnguage InnerNode)-- 
                                //XmlElement LblStrtDXm4 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("LblStrtDXm4"));
                                //LblStrtDXm4.SetAttribute("czVersion", "Vybrané počáteční datum: ");
                                //LblStrtDXm4.SetAttribute("enVersion", "Selected start date: ");
                            }//LblStrtDXm4-(Lnguage InnerNode)

                            {//XmMnth1-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement XmMnth1 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("XmMnth1"));
                                XmMnth1.SetAttribute("czVersion", "1 měsíc");
                                XmMnth1.SetAttribute("enVersion", "1 Month");
                            }//XmMnth1-(Lnguage InnerNode)
                            {//XmMnth3-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement XmMnth3 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("XmMnth3"));
                                XmMnth3.SetAttribute("czVersion", "3 měsíce");
                                XmMnth3.SetAttribute("enVersion", "3 Months");
                            }//XmMnth3-(Lnguage InnerNode)
                            {//XmMnth6-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement XmMnth6 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("XmMnth6"));
                                XmMnth6.SetAttribute("czVersion", "6 měsíců");
                                XmMnth6.SetAttribute("enVersion", "6 Months");
                            }//XmMnth6-(Lnguage InnerNode)
                            {//XmMnth12-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement XmMnth12 = (XmlElement)Lnguage.AppendChild(docc.CreateElement("XmMnth12"));
                                XmMnth12.SetAttribute("czVersion", "1 rok");
                                XmMnth12.SetAttribute("enVersion", "1 Year");
                            }//XmMnth12-(Lnguage InnerNode)
                            //
                            {//XmMnthAll-(Lnguage InnerNode)-- "Requested data are not available"
                                XmlElement XmMnthAll = (XmlElement)Lnguage.AppendChild(docc.CreateElement("XmMnthAll"));
                                XmMnthAll.SetAttribute("czVersion", "Vše");
                                XmMnthAll.SetAttribute("enVersion", "All");
                            }//XmMnthAll-(Lnguage InnerNode)
                            // Save the document to a file. White space is
                            // preserved (no white space).
                            docc.PreserveWhitespace = true;
                            docc.Save("Languages.xml");
                        }//Language.xml create & save
                    }//Language.xml create & save
                    {//XmlDocument load "ExchangeRateChecker.xml"
                        XmlDocument dcLoad = new XmlDocument();
                        //dcLoad.Load("ExchangeRateChecker.xml");
                        if (File.Exists(Application.StartupPath + @"\ExchangeRateChecker.xml"))
                        {

                            dcLoad.Load(Application.StartupPath + @"\ExchangeRateChecker.xml");

                            XmlNodeList ExracheNodes = dcLoad.DocumentElement.ChildNodes;
                            foreach (XmlNode nodeOne in ExracheNodes)
                            {
                                if (nodeOne.InnerText == "SOCPXmlInt")
                                {

                                    //MessageBox.Show("Name: " + nodeOne.Name.ToString());
                                    ////MessageBox.Show("Value: " + nodeOne.Value.ToString());
                                    //MessageBox.Show("InnerText: " + nodeOne.InnerText.ToString());
                                    //MessageBox.Show("ChildNodes.Count: " + nodeOne.ChildNodes.Count.ToString());
                                    //MessageBox.Show("BaseURI: " + nodeOne.BaseURI.ToString());
                                    //MessageBox.Show("FirstChild.InnerText: " + nodeOne.FirstChild.InnerText.ToString());
                                    //MessageBox.Show("InnerXml: "+nodeOne.InnerXml.ToString());
                                    //MessageBox.Show("Attributes.Count: " + nodeOne.Attributes.Count.ToString());
                                    string nodeAtt = "";
                                    foreach (XmlAttribute xmlAttr in nodeOne.Attributes)
                                    {
                                        if (int.TryParse(xmlAttr.InnerText, out ShwOneCrrncsPosition))
                                        {
                                            int.TryParse(xmlAttr.InnerText, out ShwOneCrrncsPosition);

                                        }
                                        nodeAtt += xmlAttr.InnerText + Environment.NewLine;
                                        nodeAtt += xmlAttr.Name + Environment.NewLine;
                                    }
                                    //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                }
                                if (nodeOne.InnerText == "LNGXmlInt")
                                {

                                    //MessageBox.Show("Name: " + nodeOne.Name.ToString());
                                    ////MessageBox.Show("Value: " + nodeOne.Value.ToString());
                                    //MessageBox.Show("InnerText: " + nodeOne.InnerText.ToString());
                                    //MessageBox.Show("ChildNodes.Count: " + nodeOne.ChildNodes.Count.ToString());
                                    //MessageBox.Show("BaseURI: " + nodeOne.BaseURI.ToString());
                                    //MessageBox.Show("FirstChild.InnerText: " + nodeOne.FirstChild.InnerText.ToString());
                                    //MessageBox.Show("InnerXml: "+nodeOne.InnerXml.ToString());
                                    //MessageBox.Show("Attributes.Count: " + nodeOne.Attributes.Count.ToString());
                                    string nodeAtt = "";
                                    foreach (XmlAttribute xmlAttr in nodeOne.Attributes)
                                    {
                                        if (int.TryParse(xmlAttr.InnerText, out LngSetting))
                                        {
                                            int.TryParse(xmlAttr.InnerText, out LngSetting);

                                        }
                                        nodeAtt += xmlAttr.InnerText + Environment.NewLine;
                                        nodeAtt += xmlAttr.Name + Environment.NewLine;
                                    }
                                    //MessageBox.Show(LngSetting.ToString());

                                }
                            }
                            //MessageBox.Show("That file exists already.");
                            //return;
                        }
                    }//XmlDocument load "ExchangeRateChecker.xml"

                    switch (LngSetting)
                    {
                        case 0://Czech
                            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("cz");
                            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("cz");
                            {//XmlDocument load "Languages.xml"
                                XmlDocument dcLoad = new XmlDocument();
                                //dcLoad.Load("Languages.xml");
                                if (File.Exists(Application.StartupPath + @"\Languages.xml"))
                                {

                                    dcLoad.Load(Application.StartupPath + @"\Languages.xml");

                                    XmlNodeList LngNodes = dcLoad.DocumentElement.ChildNodes;
                                    foreach (XmlNode nodeOne in LngNodes)
                                    {
                                        //HntLstDataRefresh

                                        if (nodeOne.Name == "HntLstDataRefresh")
                                        {

                                            foreach (XmlAttribute HntLstDataRefreshCZ in nodeOne.Attributes)
                                            {
                                                if (HntLstDataRefreshCZ.Name == "czVersion")
                                                {
                                                    HntLstDataRefresh = HntLstDataRefreshCZ.InnerText;

                                                }
                                            }
                                            
                                        }
                                        if (nodeOne.Name == "Currency")
                                        {

                                            //MessageBox.Show("Name: " + nodeOne.Name.ToString());
                                            ////MessageBox.Show("Value: " + nodeOne.Value.ToString());
                                            //MessageBox.Show("InnerText: " + nodeOne.InnerText.ToString());
                                            //MessageBox.Show("ChildNodes.Count: " + nodeOne.ChildNodes.Count.ToString());
                                            //MessageBox.Show("BaseURI: " + nodeOne.BaseURI.ToString());
                                            //MessageBox.Show("FirstChild.InnerText: " + nodeOne.FirstChild.InnerText.ToString());
                                            //MessageBox.Show("InnerXml: "+nodeOne.InnerXml.ToString());
                                            //MessageBox.Show("Attributes.Count: " + nodeOne.Attributes.Count.ToString());
                                            //string nodeAtt = "";
                                            foreach (XmlAttribute crrCZ in nodeOne.Attributes)
                                            {
                                                if (crrCZ.Name== "czVersion")
                                                {
                                                    TtlXm = crrCZ.InnerText;

                                                }
                                                //nodeAtt += xmlAttr.InnerText + Environment.NewLine;
                                                //nodeAtt += xmlAttr.Name + Environment.NewLine;
                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "NwCrrTxt")
                                        {

                                            foreach (XmlAttribute NwCrrTxtCZ in nodeOne.Attributes)
                                            {
                                                if (NwCrrTxtCZ.Name == "czVersion")
                                                {
                                                    NwCrrTxtXm = NwCrrTxtCZ.InnerText;

                                                }
                                                
                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "BttLngXm")
                                        {

                                            foreach (XmlAttribute BttLngXmCZ in nodeOne.Attributes)
                                            {
                                                if (BttLngXmCZ.Name == "czVersion")
                                                {
                                                    BttLngXm = BttLngXmCZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        
                                        if (nodeOne.Name == "BttStrtDXm")
                                        {

                                            foreach (XmlAttribute BttStrtDXmCZ in nodeOne.Attributes)
                                            {
                                                if (BttStrtDXmCZ.Name == "czVersion")
                                                {
                                                    BttStrtDXm = BttStrtDXmCZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "BttEndDXm")
                                        {
                                            
                                            foreach (XmlAttribute BttEndDXmCZ in nodeOne.Attributes)
                                            {
                                                if (BttEndDXmCZ.Name == "czVersion")
                                                {
                                                    BttEndDXm = BttEndDXmCZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        
                                        if (nodeOne.Name == "LblStrtDXm0")
                                        {

                                            foreach (XmlAttribute LblStrtDXm0CZ in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm0CZ.Name == "czVersion")
                                                {
                                                    LblStrtDXm0 = LblStrtDXm0CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm1")
                                        {

                                            foreach (XmlAttribute LblStrtDXm1CZ in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm1CZ.Name == "czVersion")
                                                {
                                                    LblStrtDXm1 = LblStrtDXm1CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm2")
                                        {

                                            foreach (XmlAttribute LblStrtDXm2CZ in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm2CZ.Name == "czVersion")
                                                {
                                                    LblStrtDXm2 = LblStrtDXm2CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm3")
                                        {

                                            foreach (XmlAttribute LblStrtDXm3CZ in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm3CZ.Name == "czVersion")
                                                {
                                                    LblStrtDXm3 = LblStrtDXm3CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "HntCrr0")
                                        {

                                            foreach (XmlAttribute HntCrr0CZ in nodeOne.Attributes)
                                            {
                                                if (HntCrr0CZ.Name == "czVersion")
                                                {
                                                    HntCrr0 = HntCrr0CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "HntCrr1")
                                        {

                                            foreach (XmlAttribute HntCrr1CZ in nodeOne.Attributes)
                                            {
                                                if (HntCrr1CZ.Name == "czVersion")
                                                {
                                                    HntCrr1 = HntCrr1CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth1")
                                        {

                                            foreach (XmlAttribute XmMnth1CZ in nodeOne.Attributes)
                                            {
                                                if (XmMnth1CZ.Name == "czVersion")
                                                {
                                                    XmMnth1 = XmMnth1CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth3")
                                        {

                                            foreach (XmlAttribute XmMnth3CZ in nodeOne.Attributes)
                                            {
                                                if (XmMnth3CZ.Name == "czVersion")
                                                {
                                                    XmMnth3 = XmMnth3CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth6")
                                        {

                                            foreach (XmlAttribute XmMnth6CZ in nodeOne.Attributes)
                                            {
                                                if (XmMnth6CZ.Name == "czVersion")
                                                {
                                                    XmMnth6= XmMnth6CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth12")
                                        {

                                            foreach (XmlAttribute XmMnth12CZ in nodeOne.Attributes)
                                            {
                                                if (XmMnth12CZ.Name == "czVersion")
                                                {
                                                    XmMnth12 = XmMnth12CZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnthAll")
                                        {

                                            foreach (XmlAttribute XmMnthAllCZ in nodeOne.Attributes)
                                            {
                                                if (XmMnthAllCZ.Name == "czVersion")
                                                {
                                                    XmMnthAll = XmMnthAllCZ.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        //
                                    }
                                }
                            }//XmlDocument load "Languages.xml"
                            
                            break;
                        case 1://English
                            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en");

                            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");

                            {//XmlDocument load "Languages.xml"
                                XmlDocument dcLoad = new XmlDocument();
                                //dcLoad.Load("Languages.xml");
                                if (File.Exists(Application.StartupPath + @"\Languages.xml"))
                                {

                                    dcLoad.Load(Application.StartupPath + @"\Languages.xml");

                                    XmlNodeList LngNodes = dcLoad.DocumentElement.ChildNodes;
                                    foreach (XmlNode nodeOne in LngNodes)
                                    {
                                        if (nodeOne.Name == "HntLstDataRefresh")
                                        {

                                            foreach (XmlAttribute HntLstDataRefreshEN in nodeOne.Attributes)
                                            {
                                                if (HntLstDataRefreshEN.Name == "enVersion")
                                                {
                                                    HntLstDataRefresh = HntLstDataRefreshEN.InnerText;

                                                }
                                            }

                                        }
                                        if (nodeOne.Name == "Currency")
                                        {

                                            //MessageBox.Show("Name: " + nodeOne.Name.ToString());
                                            ////MessageBox.Show("Value: " + nodeOne.Value.ToString());
                                            //MessageBox.Show("InnerText: " + nodeOne.InnerText.ToString());
                                            //MessageBox.Show("ChildNodes.Count: " + nodeOne.ChildNodes.Count.ToString());
                                            //MessageBox.Show("BaseURI: " + nodeOne.BaseURI.ToString());
                                            //MessageBox.Show("FirstChild.InnerText: " + nodeOne.FirstChild.InnerText.ToString());
                                            //MessageBox.Show("InnerXml: "+nodeOne.InnerXml.ToString());
                                            //MessageBox.Show("Attributes.Count: " + nodeOne.Attributes.Count.ToString());
                                            //string nodeAtt = "";
                                            foreach (XmlAttribute crrEN in nodeOne.Attributes)
                                            {
                                                if (crrEN.Name == "enVersion")
                                                {
                                                    TtlXm = crrEN.InnerText;

                                                }
                                                //nodeAtt += xmlAttr.InnerText + Environment.NewLine;
                                                //nodeAtt += xmlAttr.Name + Environment.NewLine;
                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "NwCrrTxt")
                                        {

                                            foreach (XmlAttribute NwCrrTxtEN in nodeOne.Attributes)
                                            {
                                                if (NwCrrTxtEN.Name == "enVersion")
                                                {
                                                    NwCrrTxtXm = NwCrrTxtEN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "BttLngXm")
                                        {

                                            foreach (XmlAttribute BttLngXmEN in nodeOne.Attributes)
                                            {
                                                if (BttLngXmEN.Name == "enVersion")
                                                {
                                                    BttLngXm = BttLngXmEN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        
                                        if (nodeOne.Name == "BttStrtDXm")
                                        {

                                            foreach (XmlAttribute BttStrtDXmEN in nodeOne.Attributes)
                                            {
                                                if (BttStrtDXmEN.Name == "enVersion")
                                                {
                                                    BttStrtDXm = BttStrtDXmEN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "BttEndDXm")
                                        {
                                            
                                            foreach (XmlAttribute BttEndDXmEN in nodeOne.Attributes)
                                            {
                                                if (BttEndDXmEN.Name == "enVersion")
                                                {
                                                    BttEndDXm = BttEndDXmEN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        
                                        if (nodeOne.Name == "LblStrtDXm0")
                                        {

                                            foreach (XmlAttribute LblStrtDXm0EN in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm0EN.Name == "enVersion")
                                                {
                                                    LblStrtDXm0 = LblStrtDXm0EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm1")
                                        {

                                            foreach (XmlAttribute LblStrtDXm1EN in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm1EN.Name == "enVersion")
                                                {
                                                    LblStrtDXm1 = LblStrtDXm1EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm2")
                                        {

                                            foreach (XmlAttribute LblStrtDXm2EN in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm2EN.Name == "enVersion")
                                                {
                                                    LblStrtDXm2 = LblStrtDXm2EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "LblStrtDXm3")
                                        {

                                            foreach (XmlAttribute LblStrtDXm3EN in nodeOne.Attributes)
                                            {
                                                if (LblStrtDXm3EN.Name == "enVersion")
                                                {
                                                    LblStrtDXm3 = LblStrtDXm3EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "HntCrr0")
                                        {

                                            foreach (XmlAttribute HntCrr0EN in nodeOne.Attributes)
                                            {
                                                if (HntCrr0EN.Name == "enVersion")
                                                {
                                                    HntCrr0 = HntCrr0EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "HntCrr1")
                                        {

                                            foreach (XmlAttribute HntCrr1EN in nodeOne.Attributes)
                                            {
                                                if (HntCrr1EN.Name == "enVersion")
                                                {
                                                    HntCrr1 = HntCrr1EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth1")
                                        {

                                            foreach (XmlAttribute XmMnth1EN in nodeOne.Attributes)
                                            {
                                                if (XmMnth1EN.Name == "enVersion")
                                                {
                                                    XmMnth1 = XmMnth1EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth3")
                                        {

                                            foreach (XmlAttribute XmMnth3EN in nodeOne.Attributes)
                                            {
                                                if (XmMnth3EN.Name == "enVersion")
                                                {
                                                    XmMnth3 = XmMnth3EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth6")
                                        {

                                            foreach (XmlAttribute XmMnth6EN in nodeOne.Attributes)
                                            {
                                                if (XmMnth6EN.Name == "enVersion")
                                                {
                                                    XmMnth6 = XmMnth6EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnth12")
                                        {

                                            foreach (XmlAttribute XmMnth12EN in nodeOne.Attributes)
                                            {
                                                if (XmMnth12EN.Name == "enVersion")
                                                {
                                                    XmMnth12 = XmMnth12EN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                        if (nodeOne.Name == "XmMnthAll")
                                        {

                                            foreach (XmlAttribute XmMnthAllEN in nodeOne.Attributes)
                                            {
                                                if (XmMnthAllEN.Name == "enVersion")
                                                {
                                                    XmMnthAll = XmMnthAllEN.InnerText;

                                                }

                                            }
                                            //MessageBox.Show(ShwOneCrrncsPosition.ToString());

                                        }
                                    }

                                }

                            }//XmlDocument load "Languages.xml"



                            break;
                        default:
                            break;
                    }
                }
            }//Manager stuff
            {//Program stuff
                {

                    ScSize();
                    SniffIt(); ShowOneCrrncy(CrrncsRates);
                    //ShowOneCrrncy(CrrncsRates);
                    tcker.Tick += (sender, e) =>
                    {
                        DateTime now = DateTime.Now;
                        //Text = now.ToLongTimeString();

                        if (!updateWatcher)
                        {
                            updateWatcher = true;
                            if (tickCounter == 150)
                            {
                                tickCounter = 0;
                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                            }
                            else tickCounter++;
                            updateWatcher = false;
                        }
                    };
                    //tcker.Start();

                    IntrScr();
                }
            }//Program stuff

        }
        private int phase = 0;
        public int Phase
        {
            get { return phase; }
            set
            {
                phase = value;

                if (phase > 1) phase = 0;
                switch (phase)
                {

                    case 1:

                        {
                            nvrtd.Clear();
                            decimal cnstLcl = new decimal();
                            cnstLcl = 1 / 4 + 2;
                            if (prcDateTemporary.Count > 1)
                            {
                                foreach (var prcData in prcDateTemporary)
                                {
                                    nvrtd.Add(new PriceData(prcData.Date, 1 / prcData.Price));
                                }
                                if (nvrtd.Count > 1)
                                {
                                    //PriceList.Clear();
                                    PriceList = nvrtd;
                                    DrawGraph();
                                    txtStartDate.Text = nvrtd[0].Date.ToString() + Environment.NewLine + nvrtd[0].Price.ToString();
                                    txtEndDate.Text = nvrtd[nvrtd.Count - 1].Date.ToString() + Environment.NewLine + nvrtd[nvrtd.Count - 1].Price.ToString();
                                    Ttle.Text = "EUR - € vs " + Ttle.Text;
                                }
                            }
                            else
                            {
                                if (CrrncyHstry.Count > 1)
                                {
                                    foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                                    {
                                        nvrtd.Add(new PriceData(kvp.Key, cnstLcl - kvp.Value));
                                    }
                                    if (nvrtd.Count > 1)
                                    {
                                        PriceList = nvrtd;
                                        DrawGraph();
                                        txtStartDate.Text = nvrtd[0].Date.ToString() + Environment.NewLine + nvrtd[0].Price.ToString();
                                        txtEndDate.Text = nvrtd[nvrtd.Count - 1].Date.ToString() + Environment.NewLine + nvrtd[nvrtd.Count - 1].Price.ToString();
                                        Ttle.Text = "EUR - € vs " + Ttle.Text;
                                    }
                                }
                            }
                        }//invert

                        break;
                    case 0:

                        {
                            nvrtd.Clear();
                            if (prcDateTemporary.Count > 1)
                            {
                                PriceList = prcDateTemporary;
                                DrawGraph();
                                txtStartDate.Text = prcDateTemporary[0].Date.ToString() + Environment.NewLine + prcDateTemporary[0].Price.ToString();
                                txtEndDate.Text = prcDateTemporary[prcDateTemporary.Count - 1].Date.ToString() + Environment.NewLine + prcDateTemporary[prcDateTemporary.Count - 1].Price.ToString();
                                Ttle.Text = CrrncsRates.ElementAt(ShwOneCrrncsPosition).Key;
                            }
                            else
                            {
                                if (CrrncyHstry.Count > 1)
                                {
                                    PriceList.Clear();
                                    foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                                    {
                                        PriceList.Add(new PriceData(kvp.Key, kvp.Value));
                                    }
                                    txtStartDate.Text = PriceList[0].Date.ToString() + Environment.NewLine + PriceList[0].Price.ToString();
                                    txtEndDate.Text = PriceList[PriceList.Count - 1].Date.ToString() + Environment.NewLine + PriceList[PriceList.Count - 1].Price.ToString();
                                    Ttle.Text = CrrncsRates.ElementAt(ShwOneCrrncsPosition).Key;
                                    DrawGraph();
                                }
                            }
                        }//reset

                        break;
                    default:
                        break;
                }

            }
        }
        
        //FUNCTION - private-------------------        
        private void ScSize()
        {
            //{
            //    //SetBounds((Screen.GetBounds(this).Width / 2) - (Width / 2), (Screen.GetBounds(this).Height / 2) - (Height / 2), Width, Height, BoundsSpecified.Location);
            //    //Set Form1(Window) fit on any Monitor             
            //    Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            //    int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 2;
            //    int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 2;
            //    Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            //    //thiCnst.S.Size = new Size(w, h);
            //    Size = new Size(w - ((1 / 2) / 2), h - ((1 / 2) / 2));
            //    //-----------------Set Form1(Window) fit on any Monitor 
            //}





        }
        private void IntrScr()
        {
            ExRaCheDsgn();

            Prgrm.Visible = true;
            Prgrm.BringToFront();

            try
            {
                //Intro Screen

                Cnst.S.scIntrBackground.Location = new Point(0, 0);
                Cnst.S.scIntrBackground.BackColor = Color.Black;
                Cnst.S.scIntrBackground.Size = new Size(ClientSize.Width, ClientSize.Height);
                Cnst.S.scIntrBackground.Dock = DockStyle.Fill;
                //Cnst.S.scIntrBackground.BringToFront();
                DrwCore.Location = new Point(19, 16);
                DrwCore.Height = ClientSize.Height - 17 - 20;
                DrwCore.Width = ClientSize.Width - 17 - 20;
                DrwCore.BackColor = ClrdBck;
                DrwCore.Dock = DockStyle.Fill;
                DrwCore.MouseUp += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Right && Tckr1.Enabled == true)
                    {
                        TmOfClick = DateTime.Now;
                        if (TmrCnvertToIntEnd > (int)TmOfClick.Ticks)
                        {
                            //DrwCore.Dispose();
                            //Prgrm.Visible = true;
                            Prgrm.BringToFront();
                            //Cnst.S.scIntrBackground.SendToBack();
                            //Prgrm.BringToFront();
                            Cnst.S.scIntrBackground.Hide();
                        }
                        //MessageBox.Show(TmerStart.ToString());
                        //MessageBox.Show(TmerEnd.ToString());
                        //MessageBox.Show(TmrCnvertToIntEnd.ToString());
                    }


                };
                Cnst.S.scIntrBackground.Controls.Add(DrwCore);




                Controls.Add(Cnst.S.scIntrBackground);
                Cnst.S.scIntrBackground.BringToFront();
                Prgrm.SendToBack();
                //NEW CHANGE
                {//Draw some Controls


                    Random mManager = new Random();
                    int fnt = 17;
                    Font fntMine = new Font("Litograph", fnt, FontStyle.Bold);
                    string[] fill = { "Exchange", "rate", "checker", "Exchange", "rate", "checker", "Exchange", "rate", "checker", "Exchange", "rate", "checker" };

                    int rndCreate = mManager.Next(0, (fill.Length - 1));
                    for (int i = 0; i < rndCreate; i++)
                    {
                        string tXtI = fill[mManager.Next(rndCreate)];
                        IntroStrngs.Add(tXtI);
                    }


                    for (int i = 0; i < IntroStrngs.Count; i++)
                    {




                        Random rndm = new Random();



                        LblAnmtd sssAnmtd = new LblAnmtd(new Point(i * 30, i * 30), DrwCore, IntroStrngs[i]);

                        sssAnmtd.Font = fntMine;
                        DrwCore.Controls.Add(sssAnmtd);
                        {//Create new Move2D for later execution
                            { //Create new Move2D for later execution

                                LblAnmtd.Move2D animationON = new LblAnmtd.Move2D(rndm.Next(23, 45), rndm.Next(23, 45));
                                sssAnmtd.ReceiveMove(animationON, 33);
                                //e.Graphics.DrawString(sssAnmtd.Text, mgnr.Font, new SolidBrush(Color.FromArgb(rndm.Next(50, 255), rndm.Next(0, 255), rndm.Next(25, 255))), sssAnmtd.Location.X + (txtSz2.Width / 2) + i * 10, sssAnmtd.Location.Y + (txtSz2.Height / 2) + i * 10);

                            }//Create new Move2D for later execution

                        }//Create new Move2D for later execution
                    }
                }//Draw some Controls





                Cnst.S.scIntrBackground.Paint += (sender, e) =>
                {
                    //{////ClientResultsDraw
                    //    GraphicsContainer Brdr = e.Graphics.BeginContainer();//BaseLines Container
                    //    int w = ClientSize.Width-28, h = ClientSize.Height - 28;
                    //    Pen pncl = new Pen(Color.DarkOrange, 7f);
                    //    Pen pnclDash = new Pen(ForeColor, 3f);

                    //    pnclDash.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };
                    //    SpwLc = new Point(0, 0);
                    //    e.Graphics.ScaleTransform(1.0f, -1.0f);
                    //    e.Graphics.TranslateTransform(14, 56 - Height);
                    //    Rectangle rmck = new Rectangle();
                    //    rmck.Height = h;
                    //    rmck.Width = w;//funguje 
                    //    e.Graphics.DrawRectangle(pncl, rmck);


                    //    e.Graphics.EndContainer(Brdr);//ContainerEnd////BaseLines Container


                    //}//ClientResultsDraw



                };

                //Draw some Controls

                {//Print
                    // FINALLY PRINTSCR All spawned Items DONE!!!!!!!! BECOUSE I AM PRINTING A JUST PANEL NOT FORM1 (remeber this!) $$
                    Bitmap smaBmp = new Bitmap(DrwCore.Width, DrwCore.Height);
                    DrwCore.DrawToBitmap(smaBmp, new Rectangle(Point.Empty, smaBmp.Size));

                    string myPath = Application.StartupPath;
                    myPath += "\\Intro Screen.jpg";
                    //MessageBox.Show(myPath);
                    try//Save print
                    { smaBmp.Save(myPath, ImageFormat.Jpeg); }
                    catch (Exception) { }//Save print
                }//Print+Save
                //NEW CHANGE----

                {//PictureBox
                    PictureBox scIntr = new PictureBox();
                    scIntr.Location = new Point(0, 0);
                    if (ClientSize.Width > ClientSize.Height)
                    {
                        scIntr.Size = new Size(ClientSize.Height, ClientSize.Height);
                        scIntr.Location = new Point((ClientSize.Width - scIntr.Width) / 2, 0);
                    }
                    else
                    {
                        scIntr.Size = new Size(ClientSize.Width, ClientSize.Width);
                    }
                    scIntr.BackColor = Color.BlueViolet;
                    scIntr.SizeMode = PictureBoxSizeMode.StretchImage;
                    string autoPathScr = Application.StartupPath;
                    //---Some Changes---------
                    autoPathScr += "\\Intro Screen.jpg";//New Load
                                                        //autoPathScr += "\\IntrScr\\IntrScrZeuCnst.S.jpg";//Old Load
                                                        //MessageBox.Show(autoPathScr);
                    try
                    {
                        scIntr.Image = Image.FromFile(autoPathScr);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }//Load the picture
                    scIntr.BringToFront();
                    //Controls.Add(scIntr);

                }//PictureBox




                //scIntr.Hide();
                Tckr1.Interval = 3500;
                Tckr1.Start();
                TmerStart = DateTime.Now;
                TmrCnvertToInt = (int)TmerStart.Ticks;
                TmerEnd = TmerStart.AddSeconds(3).AddMilliseconds(500);
                TmrCnvertToIntEnd = (int)TmerEnd.Ticks;
                ClctIntrval = ((int)TmerEnd.Ticks - (int)TmerStart.Ticks) / 10000;
                {//Execute && Update Move2D
                    Cnst.S.Ticker.Interval = 833;
                    Cnst.S.Ticker.Tick += (senderr, r) =>
                    {


                        foreach (LblAnmtd lbl in Cnst.S.Movement2D)
                        {
                            lbl.Ticker_Tick2();
                        }
                        Cnst.S.Movement2D.RemoveAll(lbl => lbl.isMoving == false);
                        if (Cnst.S.Movement2D.Count == 0) Cnst.S.Ticker.Stop();
                        //IntersectBlocks();
                        //Cnst.S.scIntrBackground.Refresh();
                        DrwCore.Refresh();
                    };
                }//Execute && Update Move2D

                Tckr1.Tick += (sender, e) =>
                {

                    //scIntr.Hide();

                    //Prgrm.Visible = true;
                    Prgrm.BringToFront();
                    //Cnst.S.scIntrBackground.SendToBack();

                    Cnst.S.scIntrBackground.Hide();

                    Tckr1.Stop();

                    foreach (Control t in Controls)
                    {
                        //Font = new Font("Cambria", FntSize, FontStyle.Bold);
                        BackColor = ClrdFrnt;//ClrdBck;
                        ForeColor = ClrdBck;//ClrdFrnt;
                    }

                    {//DirectoryCreation
                     // Specify the directory you want to manipulate.
                        string myPath = Application.StartupPath;
                        myPath += "\\User save";
                        try
                        {
                            // Determine whether the directory existCnst.S.
                            if (Directory.Exists(myPath))
                            {
                                //MessageBox.Show("That path exists already.");
                                return;
                            }

                            // Try to create the directory.
                            DirectoryInfo di = Directory.CreateDirectory(myPath);
                            //MessageBox.Show("The directory was created successfully at {0}.", Directory.GetCreationTime(myPath).ToLongDateString());

                            // Delete the directory.
                            //di.Delete();
                            //Console.WriteLine("The directory was deleted successfully.");
                        }
                        catch (Exception z)
                        {
                            MessageBox.Show("The process failed: {0}", z.ToString());
                        }
                        finally { }

                    }//DirectoryCreation

                };//Intro Screen
            }
            catch (Exception) { }//IntroScr
        }//Startup seting for window
        public void SniffIt()
        {
            {//SniffIt
                {//SniffIt
                    string url = "http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml";
                    XDocument doc = XDocument.Load(url);

                    XNamespace gesmes = "http://www.gesmes.org/xml/2002-08-01";
                    XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

                    var cubes = doc.Descendants(ns + "Cube")
                                   .Where(x => x.Attribute("currency") != null)
                                   .Select(x => new {
                                       Currency = (string)x.Attribute("currency"),
                                       Rate = (decimal)x.Attribute("rate")
                                   });
                    //string otpt = "";
                    CrrncsRates.Clear();
                    foreach (var result in cubes)
                    {

                        CrrncsRates.Add(result.Currency, result.Rate);
                        //otpt += result.Currency + ": " + result.Rate.ToString() + Environment.NewLine;
                        //Console.WriteLine("{0}: {1}", result.Currency, result.Rate);
                    }
                    //MessageBox.Show(otpt);
                    //MessageBox.Show(CrrncsRates.Count.ToString());
                    if (CrrncsRates.Count > 0)
                    {
                        //{//SniffGraphData
                        //    {//SniffGraphData
                        //        string urlGrph = "https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/usd.xml";
                        //        XDocument docGrph = XDocument.Load(urlGrph);
                        //        //XElement obs = docGrph.Descendants().Where(x => x.Name.LocalName == "Obs").FirstOrDefault();

                        //        //string TIME_PERIOD = obs.Attribute("TIME_PERIOD").Value;
                        //        //string OBS_VALUE = obs.Attribute("OBS_VALUE").Value;
                        //        //MessageBox.Show(OBS_VALUE);
                        //        //MessageBox.Show(TIME_PERIOD);

                        //        XNamespace xsi = "http://www.ecb.europa.eu/vocabulary/stats/exr/1 https://stats.ecb.europa.eu/stats/vocabulary/exr/1/2006-09-04/sdmx-compact.xsd";

                        //        var obss = docGrph.Descendants()
                        //           .Where(x => x.Attribute("TIME_PERIOD") != null)
                        //           .Select(x => new {
                        //               TIME_PERIOD = (DateTime)x.Attribute("TIME_PERIOD"),
                        //               OBS_VALUE = (decimal)x.Attribute("OBS_VALUE")
                        //           });
                        //        otpt = "";
                        //        CrrncyHstry.Clear();
                        //        //PriceList.Clear();
                        //        foreach (var result in obss)
                        //        {
                        //            CrrncyHstry.Add(result.TIME_PERIOD, result.OBS_VALUE);
                        //            //CrrncsRates.Add(result.Currency, result.Rate);
                        //            otpt += result.TIME_PERIOD.ToString() + ": " + result.OBS_VALUE.ToString() + Environment.NewLine;
                        //            //Console.WriteLine("{0}: {1}", result.Currency, result.Rate);
                        //            //MessageBox.Show(otpt);
                        //        }


                        //    }//SniffGraphData
                        //}//SniffGraphData

                        //PriceList.Clear();
                        //foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                        //{
                        //    PriceList.Add(new PriceData(kvp.Key, kvp.Value));
                        //}

                        //ShowOneCrrncy(CrrncsRates);

                    }
                }//SniffIt
            }//SniffIt0
            //MessageBox.Show(doccxml.OuterXml);
        }
        private void ShowOneCrrncy(Dictionary<string, decimal> crrncsRts)
        {
            string mnts = "";
            if (DateTime.Now.Minute < 10)
            {

                mnts = "0" + DateTime.Now.Minute.ToString();
                //MessageBox.Show(mnts);
            }
            else
            {
                mnts = DateTime.Now.Minute.ToString();
            }
            string scnds = "";

            if (DateTime.Now.Second < 10)
            {
                scnds = "0" + (DateTime.Now.Second.ToString());
                //MessageBox.Show(scnds);
            }
            else
            {
                scnds = DateTime.Now.Second.ToString();
            }
            LstDataRefresh.Text = DateTime.Now.Hour.ToString() + ":" + mnts + ":" + scnds;

            //LstDataRefresh.Text = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            LstDataRefresh.Invalidate();

            Ttle.Text = crrncsRts.ElementAt(ShwOneCrrncsPosition).Key;
            Icn.Text = "http://www.ecb.europa.eu/shared/img/flags/" + Ttle.Text + icn.Text;

            //NCrr.Text = crrncsRts.ElementAt(ShwOneCrrncsPosition).Value.ToString() + " " + Ttle.Text + " buys 1 *** - " + DateTime.Now.ToString(/*"MM/dd/yyyy"*/);
            UrlImg.Text = "";
            //CreateChart(Ttle.Text);
            //shwPicture.Controls.Add(picGraph);


            {//SniffGraphData
                {//SniffGraphData
                    string urlGrph = "https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/" + Ttle.Text.ToLower() + ".xml";
                    XDocument docGrph = XDocument.Load(urlGrph);
                    //Series FREQ="D" CURRENCY="CZK" CURRENCY_DENOM="EUR"
                    var Series = docGrph.Descendants()
                    .Where(x => x.Attribute("CURRENCY_DENOM") != null)
                    .Select(x => new {
                        CURRENCY_DENOM = (string)x.Attribute("CURRENCY_DENOM")
                    });
                    foreach (var result in Series)
                    {
                        //CrrncyHstry.Add(result.TIME_PERIOD, result.OBS_VALUE);
                        NCrr.Text = crrncsRts.ElementAt(ShwOneCrrncsPosition).Value.ToString() + " " + Ttle.Text + " "+NwCrrTxtXm + " " + result.CURRENCY_DENOM+" "+DateTime.Now.ToString(/*"MM/dd/yyyy"*/);

                    }


                    var obss = docGrph.Descendants()
                       .Where(x => x.Attribute("TIME_PERIOD") != null)
                       .Select(x => new {
                           TIME_PERIOD = (DateTime)x.Attribute("TIME_PERIOD"),
                           OBS_VALUE = (decimal)x.Attribute("OBS_VALUE")
                       });
                    //otpt = "";
                    CrrncyHstry.Clear();
                    //PriceList.Clear();
                    foreach (var result in obss)
                    {
                        CrrncyHstry.Add(result.TIME_PERIOD, result.OBS_VALUE);
                        //CrrncsRates.Add(result.Currency, result.Rate);
                        //otpt += result.TIME_PERIOD.ToString() + ": " + result.OBS_VALUE.ToString() + Environment.NewLine;
                        //Console.WriteLine("{0}: {1}", result.Currency, result.Rate);
                        //MessageBox.Show(otpt);
                    }


                }//SniffGraphData
            }//SniffGraphData

            PriceList.Clear();
            foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
            {
                PriceList.Add(new PriceData(kvp.Key, kvp.Value));
            }
            //txtStartDate.Text = PriceList[0].Date.ToString() + Environment.NewLine + PriceList[0].Price.ToString();
            //txtEndDate.Text = PriceList[PriceList.Count - 1].Date.ToString() + Environment.NewLine + PriceList[PriceList.Count - 1].Price.ToString();

            DrawGraph();
            
            if (SrStart_date!=SrEnd_date&&SrStart_date != new DateTime(1, 1, 0001, 00, 00, 00) && SrEnd_date != new DateTime(1, 1, 0001, 00, 00, 00))
            {
                //prcDateTemporary.Clear();
                prcDateTemporary = new List<PriceData>();//! ".Clear()" isn't enough!!
                foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                {
                    if (kvp.Key == SrStart_date | kvp.Key > SrStart_date && kvp.Key == SrEnd_date | kvp.Key < SrEnd_date)
                    {
                        prcDateTemporary.Add(new PriceData(kvp.Key, kvp.Value));
                    }

                }
                if (prcDateTemporary.Count == 1)
                {
                    PriceList.Clear();
                    prcDateTemporary.Add(new PriceData(prcDateTemporary[0].Date, prcDateTemporary[0].Price));
                    //PriceList = prcDateTemporary;
                    //DrawGraph();
                    //txtStartDate.Text = prcDateTemporary[0].Date.ToString() + Environment.NewLine + prcDateTemporary[0].Price.ToString();
                    //txtEndDate.Text = prcDateTemporary[prcDateTemporary.Count - 1].Date.ToString() + Environment.NewLine + prcDateTemporary[prcDateTemporary.Count - 1].Price.ToString();
                    //ShowOneCrrncy(CrrncsRates);
                }
                if (prcDateTemporary.Count > 1)
                {
                    PriceList.Clear();
                    switch (phase)
                    {

                        case 1:

                            {
                                nvrtd.Clear();
                                decimal cnstLcl = new decimal();
                                cnstLcl = 1 / 4 + 2;
                                if (prcDateTemporary.Count > 1)
                                {
                                    foreach (var prcData in prcDateTemporary)
                                    {
                                        nvrtd.Add(new PriceData(prcData.Date, 1 / prcData.Price));
                                    }
                                    if (nvrtd.Count > 1)
                                    {
                                        //PriceList.Clear();
                                        PriceList = nvrtd;
                                        DrawGraph();
                                        txtStartDate.Text = nvrtd[0].Date.ToString() + Environment.NewLine + nvrtd[0].Price.ToString();
                                        txtEndDate.Text = nvrtd[nvrtd.Count - 1].Date.ToString() + Environment.NewLine + nvrtd[nvrtd.Count - 1].Price.ToString();
                                        Ttle.Text = "EUR - € vs " + Ttle.Text;
                                    }
                                }
                                else
                                {
                                    if (CrrncyHstry.Count > 1)
                                    {
                                        foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                                        {
                                            nvrtd.Add(new PriceData(kvp.Key, cnstLcl - kvp.Value));
                                        }
                                        if (nvrtd.Count > 1)
                                        {
                                            PriceList = nvrtd;
                                            DrawGraph();
                                            txtStartDate.Text = nvrtd[0].Date.ToString() + Environment.NewLine + nvrtd[0].Price.ToString();
                                            txtEndDate.Text = nvrtd[nvrtd.Count - 1].Date.ToString() + Environment.NewLine + nvrtd[nvrtd.Count - 1].Price.ToString();
                                            Ttle.Text = "EUR - € vs " + Ttle.Text;
                                        }
                                    }
                                }
                            }//inverted 

                            break;
                        case 0:

                            {
                                nvrtd.Clear();
                                if (prcDateTemporary.Count > 1)
                                {
                                    PriceList = prcDateTemporary;
                                    DrawGraph();
                                    txtStartDate.Text = prcDateTemporary[0].Date.ToString() + Environment.NewLine + prcDateTemporary[0].Price.ToString();
                                    txtEndDate.Text = prcDateTemporary[prcDateTemporary.Count - 1].Date.ToString() + Environment.NewLine + prcDateTemporary[prcDateTemporary.Count - 1].Price.ToString();
                                    Ttle.Text = CrrncsRates.ElementAt(ShwOneCrrncsPosition).Key;
                                }
                                else
                                {
                                    if (CrrncyHstry.Count > 1)
                                    {
                                        PriceList.Clear();
                                        foreach (KeyValuePair<DateTime, decimal> kvp in CrrncyHstry)
                                        {
                                            PriceList.Add(new PriceData(kvp.Key, kvp.Value));
                                        }
                                        txtStartDate.Text = PriceList[0].Date.ToString() + Environment.NewLine + PriceList[0].Price.ToString();
                                        txtEndDate.Text = PriceList[PriceList.Count - 1].Date.ToString() + Environment.NewLine + PriceList[PriceList.Count - 1].Price.ToString();
                                        Ttle.Text = CrrncsRates.ElementAt(ShwOneCrrncsPosition).Key;
                                        DrawGraph();
                                    }
                                }
                            }//reseted

                            break;
                        default:
                            break;
                    }
                    
                }                
            }
            else
            {
                DrawGraph();
            }
            if (Fake == false)
            {
                //shwPicture.Load(UrlImg.Text);

            }
            else
            {
                //UrlImg.Text = "";
            }

        }
        // Draw the graph.
        private Bitmap GraphBm = null;
        private void DrawGraph()
        {

            if (PriceList.Count < 1)
            {
                shwPicture.Image = null;
                WtoDMatrix = null;
                DtoWMatrix = null;
                return;
            }
            
            int wid = shwPicture.ClientSize.Width;
            int hgt = shwPicture.ClientSize.Height;
            GraphBm = new Bitmap(wid, hgt);
            using (Graphics gr = Graphics.FromImage(GraphBm))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.Clear(Color.White);
                //PixelFormat pixel = new PixelFormat();
                gr.PixelOffsetMode = PixelOffsetMode.Half;
                // Scale the data to fit.
                int wxmaxx = PriceList.Count;
                float wyminn = (float)PriceList.Min(data => data.Price);
                float wymaxx = (float)PriceList.Max(data => data.Price);
                const int margin = 10;
                float wxminn = 0;
                WtoDMatrix = MappingMatrix(
                    wxminn, wxmaxx - 1, wyminn, wymaxx,
                    margin, wid - margin, margin, hgt - margin);
                gr.Transform = WtoDMatrix;

                DtoWMatrix = WtoDMatrix.Clone();
                DtoWMatrix.Invert();

                // Draw the graph.
                using (Pen pen = new Pen(Color.Black, 0))
                {
                    // Draw tic marks.
                    PointF[] pts = { new PointF(10, 10) };
                    DtoWMatrix.TransformVectors(pts);
                    float dy = pts[0].Y;
                    float dx = pts[0].X;

                    for (int x = 0; x < PriceList.Count; x++)
                    {
                        gr.DrawLine(pen, x, wyminn, x, wyminn + dy);
                    }
                    for (int y = (int)wyminn; y <= (int)wymaxx; y++)
                    {
                        gr.DrawLine(pen, 0, y, dx, y);
                    }

                    // Get a small distance in world coordinates.
                    dx = Math.Abs(dx / 5);
                    dy = Math.Abs(dy / 5);

                    // Draw the data.
                    PointF[] points = new PointF[wxmaxx];
                    for (int i = 0; i < wxmaxx; i++)
                    {
                        float price = (float)PriceList[i].Price;
                        points[i] = new PointF(i, price);
                        gr.FillRectangle(Brushes.LimeGreen,
                            i - dx, price - dy, 2 * dx, 2 * dy);
                    }
                    pen.Color = Color.Blue;
                    gr.DrawLines(pen, points);
                }
            }

            // Display the result.
            shwPicture.Image = GraphBm;
        }
        // Return a mapping matrix.
        private Matrix MappingMatrix(
            float wxmin, float wxmax, float wymin, float wymax,
            float dxmin, float dxmax, float dymin, float dymax)
        {
            RectangleF rect = new RectangleF(
                wxmin, wymin,
                wxmax - wxmin, wymax - wymin);
            PointF[] points =
            {
                new PointF(dxmin, dymax),
                new PointF(dxmax, dymax),
                new PointF(dxmin, dymin),
            };
            return new Matrix(rect, points);
        }

        // Display the data in a tooltip.
        private int LastTipNum = -1;
        // Draw date and price lines for the mouse position.
        private void ShowDatePriceLines()
        {
            try
            {
                if (LastTipNum < 0) return;

                Bitmap bm = (Bitmap)GraphBm.Clone();
                
                using (Graphics gr = Graphics.FromImage(bm))
                {
                    gr.Transform = WtoDMatrix;
                    PriceData data = PriceList[LastTipNum];
                    using (Pen pen = new Pen(Color.Red, 0))
                    {
                        gr.DrawLine(pen, LastTipNum, 0, LastTipNum, 100000);
                        float price = (float)data.Price;
                        gr.DrawLine(pen, 0, price, 100 * PriceList.Count, price);
                    }
                }

                shwPicture.Image = bm;
                //bm.Dispose();
            }
            catch (Exception) { }

        }
        private void ExRaCheDsgn()
        {


            Size =MSzz= new Size(325, 100);
            {//Prgrm
                Prgrm.Width = ClientSize.Width;
                Prgrm.Height = ClientSize.Height;
                Prgrm.Location = new Point(0, 0);
                Prgrm.Dock = DockStyle.Fill;
                Prgrm.Visible = false;
                Controls.Add(Prgrm);
            }//Prgrm
            {//Hrzntl
                {
                    DateTime end_date = DateTime.Today;//.AddDays(-1);
                    DateTime start_date = end_date.AddMonths(-1);
                    txtStartDate.Text = start_date.ToString("dd/MM/yyyy");
                    txtEndDate.Text = end_date.ToString("dd/MM/yyyy");

                    Hrzntl.Width = ClientSize.Width;
                    Hrzntl.Height = ClientSize.Height;

                    Hrzntl.BackColor = ClrdBck;
                    Hrzntl.Dock = DockStyle.Fill;
                    Hrzntl.BringToFront();
                    Hrzntl.Controls.Clear();
                    Hrzntl.BringToFront();
                }//hrz
                {//DSG
                    
                    smtimsShown.Height = Hrzntl.Height;
                    smtimsShown.Width = Hrzntl.Width;
                    smtimsShown.Width = LstCntrl.Width - 1;
                    smtimsShown.Location = new Point(0, 0);
                    smtimsShown.Dock = DockStyle.Fill;
                    {//SomeTimesShownDSGN

                        UrlImg.TextChanged += (sender, e) =>
                        {
                            if (UrlImg.Text != "" && Fake == false)
                            {

                                shwPicture.Load(UrlImg.Text);

                            }

                        };

                        if (PriceList.Count < 1)
                        {
                            shwPicture.Image = null;
                            WtoDMatrix = null;
                            DtoWMatrix = null;
                            txtStartDate = null;
                            txtEndDate = null;
                            return;
                        }


                        shwPicture.MouseDown += (sender, e) =>
                        {
                            if (DtoWMatrix == null) return;

                            // Get the point in world coordinates.
                            PointF[] points = { new PointF(e.X, e.Y) };
                            DtoWMatrix.TransformPoints(points);
                            //points.d
                            // Get the tip number.
                            int tip_num = -1;
                            if (points[0].X >= 0) tip_num = (int)points[0].X;
                            if (tip_num >= PriceList.Count) tip_num = -1;

                            if (LastTipNum == tip_num) return;
                            LastTipNum = tip_num;
                            //Console.WriteLine(LastTipNum);

                            string tip = null;
                            if (tip_num >= 0) tip = PriceList[tip_num].ToString();
                            tipData.SetToolTip(shwPicture, tip);
                            ShowDatePriceLines();
                        };
                        shwPicture.MouseEnter += (sender, e) => 
                        { SniffIt(); };
                        shwPicture.MouseMove += (sender, e) =>
                        {
                            {
                                //tcker.Start();
                                if (DtoWMatrix == null) return;

                                // Get the point in world coordinates.
                                PointF[] points = { new PointF(e.X, e.Y) };
                                DtoWMatrix.TransformPoints(points);
                                //points.d
                                // Get the tip number.
                                int tip_num = -1;
                                if (points[0].X >= 0) tip_num = (int)points[0].X;
                                if (tip_num >= PriceList.Count) tip_num = -1;

                                if (LastTipNum == tip_num) return;
                                LastTipNum = tip_num;
                                //Console.WriteLine(LastTipNum);

                                string tip = null;
                                if (tip_num >= 0) tip = PriceList[tip_num].ToString();
                                tipData.SetToolTip(shwPicture, tip);
                                SniffIt();
                                ShowDatePriceLines();
                            }
                        };
                        shwPicture.MouseLeave += (sender, e) =>
                        {
                            //tcker.Stop();
                            SniffIt();
                            //ShowOneCrrncy(CrrncsRates);
                        };
                        tst = false;
                        if (tst)
                        {
                            //shwGraph = new GrphPanel(PriceList);
                            //smtimsShown.Controls.Add(shwGraph);
                        }
                        else
                        {
                            smtimsShown.Controls.Add(shwPicture);
                        }

                        txtStartDate.Text = PriceList[0].Date.ToString() + Environment.NewLine + PriceList[0].Price.ToString();
                        txtEndDate.Text = PriceList[PriceList.Count - 1].Date.ToString() + Environment.NewLine + PriceList[PriceList.Count - 1].Price.ToString();
                        txtStartDate.BackColor = txtEndDate.BackColor = ClrdBck;
                        txtStartDate.ForeColor = txtEndDate.ForeColor = ClrdFrnt;
                        smtimsShown.Controls.Add(txtEndDate);
                        smtimsShown.Controls.Add(txtStartDate);
                        {//LblSrSDateDsg&&LblSrEDateDsg&&Mnth-YearBUTTONS DSG
                            {
                                //Communication with user "LblSrSDate"
                                LblSrSDate = new Label() { Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleLeft, Width = smtimsShown.Width, Height = allwaysShown.Height / 6, Location = new Point(0, txtStartDate.Top - Height), BackColor = Color.FromArgb(191, ClrdFrnt), ForeColor = ClrdFrnt, Font = NfoFont };
                                if (SrStart_date != new DateTime(1, 1, 0001, 00, 00, 00))
                                {
                                    LblSrSDate.Text = SrStart_date.ToString();
                                }
                                smtimsShown.Controls.Add(LblSrSDate);//"LblSrSDate"

                                //Communication with user "LblSrEDate"
                                LblSrEDate = new Label() { Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleRight, Width = smtimsShown.Width, Height = allwaysShown.Height / 6, Location = new Point(0, txtStartDate.Bottom), BackColor = Color.FromArgb(191, ClrdFrnt), ForeColor = ClrdFrnt, Font = NfoFont };
                                if (SrEnd_date != new DateTime(1, 1, 0001, 00, 00, 00))
                                {
                                    LblSrEDate.Text = SrEnd_date.ToString();
                                }
                                smtimsShown.Controls.Add(LblSrEDate);//"LblSrEDate"

                                //BttHldrLocal-Buttons "MnthYearBUTTONS"
                                Panel BttHldrLocal = new Panel() { Dock = DockStyle.Bottom, /*TextAlign = ContentAlignment.MiddleRight, */Width = smtimsShown.Width, Height = allwaysShown.Height / 4, Location = new Point(0, txtStartDate.Bottom), BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                {//MnthYearBUTTONS-DSG
                                    {
                                        Button mnth1= new Button() {Text=XmMnth1, /*Anchor = AnchorStyles.Left | AnchorStyles.Top,*/ Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Width = (MSzz.Width-20)/5, Height = BttHldrLocal.Height, Location = new Point(0, 0), BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                        mnth1.Click += (sender, e) =>
                                        {
                                            if (Ttle.Text=="BGN")
                                            {
                                                //18.5.2004
                                                //2004,5,18
                                                SrEnd_date = new DateTime(2004, 12, 10, 00, 00, 00);//PriceList[PriceList.Count - 1].Date;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SrStart_date = SrEnd_date.AddMonths(-1);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                            else
                                            {
                                                SrStart_date = DateTime.Now.AddMonths(-1);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SrEnd_date = DateTime.Now;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                        };
                                        BttHldrLocal.Controls.Add(mnth1);

                                        Button mnth3 = new Button() { Text = XmMnth3, /*Anchor=AnchorStyles.Left|AnchorStyles.Top,*/Dock = DockStyle.Right, TextAlign = ContentAlignment.MiddleCenter, Width = (MSzz.Width - 20) / 5, Height = BttHldrLocal.Height, Location = new Point(mnth1.Left, 0), BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                        mnth3.Click += (sender, e) =>
                                        {
                                            if (Ttle.Text == "BGN")
                                            {
                                                //18.5.2004
                                                //2004,5,18
                                                SrEnd_date = new DateTime(2004, 12, 10, 00, 00, 00);//PriceList[PriceList.Count - 1].Date;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SrStart_date = SrEnd_date.AddMonths(-3);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                            else
                                            {
                                                SrStart_date = DateTime.Now.AddMonths(-3);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SrEnd_date = DateTime.Now;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                        };
                                        BttHldrLocal.Controls.Add(mnth3);

                                        Button mnth6 = new Button() { Text = XmMnth6, /*Anchor=AnchorStyles.Left|AnchorStyles.Top,*/Dock = DockStyle.Right, TextAlign = ContentAlignment.MiddleCenter, Width = (MSzz.Width - 20) / 5, Height = BttHldrLocal.Height, Location = new Point(mnth3.Left, 1), BackColor =ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                        mnth6.Click += (sender, e) =>
                                        {
                                            if (Ttle.Text == "BGN")
                                            {
                                                //18.5.2004
                                                //2004,5,18
                                                SrEnd_date = new DateTime(2004, 12, 10, 00, 00, 00);//PriceList[PriceList.Count - 1].Date;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SrStart_date = SrEnd_date.AddMonths(-6);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                            else
                                            {
                                                SrStart_date = DateTime.Now.AddMonths(-6);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SrEnd_date = DateTime.Now;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                        };
                                        BttHldrLocal.Controls.Add(mnth6);

                                        Button mnth12 = new Button() { Text = XmMnth12, /*Anchor = AnchorStyles.Right | AnchorStyles.Top, */Dock = DockStyle.Right, TextAlign = ContentAlignment.MiddleCenter, Width = (MSzz.Width - 20) / 5, Height = BttHldrLocal.Height, Location = new Point(mnth6.Left, 1), BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                        mnth12.Click += (sender, e) =>
                                        {
                                            if (Ttle.Text == "BGN")
                                            {
                                                //18.5.2004
                                                //2004,5,18
                                                SrEnd_date = new DateTime(2004, 12, 10, 00, 00, 00);//PriceList[PriceList.Count - 1].Date;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SrStart_date = SrEnd_date.AddMonths(-12);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                            else
                                            {
                                                SrStart_date = DateTime.Now.AddMonths(-12);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToShortDateString();
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SrEnd_date = DateTime.Now;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToShortDateString();
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                        };
                                        BttHldrLocal.Controls.Add(mnth12);
                                        Button mnthAll = new Button() { Text = XmMnthAll, /*Anchor = AnchorStyles.Right | AnchorStyles.Top, */Dock = DockStyle.Right, TextAlign = ContentAlignment.MiddleCenter, Width = (MSzz.Width - 20) / 5, Height = BttHldrLocal.Height, Location = new Point(mnth6.Left, 1), BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = NfoFont };
                                        mnthAll.Click += (sender, e) =>
                                        {
                                            //RstUserDates();
                                            if (nvrtd.Count>1)
                                            {
                                                SrStart_date = CrrncyHstry.ElementAt(0).Key;
                                                SrEnd_date = CrrncyHstry.ElementAt(CrrncyHstry.Count - 1).Key;
                                                DrawGraph();
                                                LblSrSDate.Text = LblStrtDXm2+SrStart_date.ToShortDateString();
                                                LblSrEDate.Text = LblStrtDXm2+SrEnd_date.ToShortDateString();
                                            }
                                            else
                                            {
                                                SrStart_date = CrrncyHstry.ElementAt(0).Key;
                                                SrEnd_date =  CrrncyHstry.ElementAt(CrrncyHstry.Count-1).Key;
                                                LblSrSDate.Text = LblStrtDXm2+SrStart_date.ToShortDateString();
                                                LblSrEDate.Text = LblStrtDXm2+SrEnd_date.ToShortDateString();
                                                DrawGraph();
                                            }
                                            //txtStartDate.Text = nvrtd[0].Date.ToString() + Environment.NewLine + nvrtd[0].Price.ToString();
                                            //txtEndDate.Text = nvrtd[nvrtd.Count - 1].Date.ToString() + Environment.NewLine + nvrtd[nvrtd.Count - 1].Price.ToString();

                                            LblSrSDate.ForeColor = ClrdBck;
                                            LblSrEDate.ForeColor = ClrdBck;
                                            SniffIt(); ShowOneCrrncy(CrrncsRates);
                                        };
                                        BttHldrLocal.Controls.Add(mnthAll);
                                        BttHldrLocal.ClientSizeChanged += (sender, e) =>
                                        {
                                            mnth1.Size = mnth3.Size = mnth6.Size = mnth12.Size = mnthAll.Size = new Size(BttHldrLocal.Width / BttHldrLocal.Controls.Count, BttHldrLocal.Height );
                                            

                                        };

                                    }
                                }//MnthYearBUTTONS-DSG
                                smtimsShown.Controls.Add(BttHldrLocal);//BttHldrLocal-Buttons "MnthYearBUTTONS"

                            }
                        }//LblSrSDateDsg&&LblSrEDateDsg&&Mnth-YearBUTTONS DSG

                    }//SomeTimesShownDSGN
                    LstCntrl = smtimsShown;
                    Hrzntl.Controls.Add(LstCntrl);
                    smtimsShown.Hide();

                    allwaysShown = new Panel();
                    allwaysShown.BackColor = ClrdBck;
                    allwaysShown.Height = Hrzntl.Height;
                    allwaysShown.Width = Hrzntl.Width;
                    allwaysShown.Location = new Point(0, 0);
                    allwaysShown.Dock = DockStyle.Fill;
                    {//AlwaysShownDSGN
                        Size szz = new Size(allwaysShown.Width - (allwaysShown.Width / DsgnModifierWdth), LstCntrl.Height);
                        Point spwn = new Point(LstCntrl.Right, LstCntrl.Top);
                        ttle = new Label() { TextAlign = ContentAlignment.MiddleCenter, Size = szz, Location = new Point(0, 0), Text = TtlXm + " " + Ttle.Text, BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = TitleFont };
                        Ttle.TextChanged += (sender, e) =>
                        {
                            ttle.Text = TtlXm+" "+Ttle.Text;
                        };
                        ttle.Dock = DockStyle.Fill;
                        
                        ttle.MouseHover += (sender, e) =>
                        {
                            
                        };
                        if (ShwNws == false)
                        {
                            ToolTip hnt = new ToolTip();
                            hnt.SetToolTip(ttle, HntCrr0);
                        }
                        if (ShwNws == true)
                        {
                            ToolTip hnt = new ToolTip();
                            hnt.SetToolTip(ttle, HntCrr1);
                        }
                        ttle.MouseDown += (sender, e) =>
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                if (ShwNws == false)
                                {
                                    {//Show
                                        ShwNws = true;

                                        smtimsShown.Dock = DockStyle.Fill;
                                        allwaysShown.Dock = DockStyle.Top;
                                        allwaysShown.Height = MSzz.Height / 2; //(MSzz.Height - F.STATUS.Height) / 2;

                                        Height = MSzz.Height * 2;// + 22;
                                        smtimsShown.Show();
                                        DrawGraph();
                                        
                                        //
                                    }//Show
                                }
                                else
                                {
                                    {//Hide
                                        ShwNws = false;
                                        smtimsShown.Invalidate();
                                        Height = MSzz.Height;// + 22;// + S.F.STATUS.Height;
                                        smtimsShown.Dock = DockStyle.Bottom;
                                        smtimsShown.Hide();
                                        allwaysShown.Height = (MSzz.Height);// - S.F.STATUS.Height);
                                        allwaysShown.Dock = DockStyle.Fill;
                                        //ttle.MouseHover += (senderf, f) =>
                                        {
                                            
                                        };
                                    }//Hide
                                }

                            }

                            if (ShwNws == false)
                            {
                                ToolTip hnt = new ToolTip();
                                hnt.SetToolTip(ttle, HntCrr0);
                            }
                            if (ShwNws == true)
                            {
                                ToolTip hnt = new ToolTip();
                                hnt.SetToolTip(ttle, HntCrr1);
                            }
                        };


                        LstCntrl = ttle;
                        allwaysShown.Controls.Add(LstCntrl);

                        {
                            //Panel btnHldr = new Panel();
                            btnHldr.Height = allwaysShown.Height;
                            btnHldr.Width = allwaysShown.Width / DsgnModifierWdth;
                            btnHldr.Location = new Point(0, 0);
                            btnHldr.BackColor = ClrdBck;
                            btnHldr.Dock = DockStyle.Left;

                            icn = new PictureBox();
                            icn.Text = ".gif";
                            icn.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                            icn.Location = new Point(0, 0);
                            icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            btnHldr.ClientSizeChanged += (sender, e) => { icn.Width = btnHldr.Width; icn.Height = btnHldr.Height / 2; };
                            icn.BackColor = ClrdBck;
                            icn.ForeColor = ClrdFrnt;
                            icn.Font = ContentFont;
                            //icn.Click += (sender, e) =>
                            //{
                            //    ToolTip hnt = new ToolTip();
                            //    hnt.SetToolTip(icn, "Icon was clicked");

                            //};
                            icn.SizeMode = PictureBoxSizeMode.Zoom;
                            Icn.TextChanged += (sender, e) =>
                            {
                                icn.Load(Icn.Text);
                            };
                            try
                            {
                                icn.Load("http://www.ecb.europa.eu/shared/img/flags/" + Ttle.Text + icn.Text);
                            }
                            catch (Exception)
                            {

                                MessageBox.Show("Wrong URL adress, icone was not found.");
                            }

                            btnHldr.Controls.Add(icn);



                            //LstDataRefresh = new Label();
                            LstDataRefresh.BackColor = ClrdBck;
                            LstDataRefresh.ForeColor = ClrdFrnt;
                            LstDataRefresh.Font = NfoFont;
                            LstDataRefresh.TextAlign = ContentAlignment.MiddleCenter;
                            string mnts = "";
                            if (DateTime.Now.Minute < 10)
                            {

                                mnts = "0" + DateTime.Now.Minute.ToString();
                                //MessageBox.Show(mnts);
                            }
                            else
                            {
                                mnts = DateTime.Now.Minute.ToString();
                            }
                            string scnds = "";

                            if (DateTime.Now.Second < 10)
                            {
                                scnds = "0" + (DateTime.Now.Second.ToString());
                                //MessageBox.Show(scnds);
                            }
                            else
                            {
                                scnds = DateTime.Now.Second.ToString();
                            }
                            LstDataRefresh.Text = DateTime.Now.Hour.ToString() + ":" + mnts + ":" + scnds;
                            LstDataRefresh.TextChanged += (sender, e) =>
                            {
                                Invalidate();
                                //ttle.Text = Ttle.Text;
                            };
                            LstDataRefresh.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                            LstDataRefresh.Location = new Point(0, icn.Bottom);
                            LstDataRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                            btnHldr.ClientSizeChanged += (sender, e) =>
                            {
                                //LstDataRefresh.Location = new Point(0, icn.Bottom);
                                //LstDataRefresh.Width = btnHldr.Width; LstDataRefresh.Height = btnHldr.Height / 2;
                            };

                            LstDataRefresh.MouseHover += (sender, e) =>
                            {
                                ToolTip hnt = new ToolTip();
                                hnt.SetToolTip(LstDataRefresh, HntLstDataRefresh);
                            };

                            btnHldr.Controls.Add(LstDataRefresh);
                            LstCntrl = btnHldr;
                            allwaysShown.Controls.Add(LstCntrl);
                        }//btnholder
                        shwSummary = new Label() { Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleCenter, Width = smtimsShown.Width, Height = smtimsShown.Height / 3, Location = new Point(0, 0), Text = NCrr.Text, BackColor = ClrdBck, ForeColor = ClrdFrnt, Font = ContentFont };
                        NCrr.TextChanged += (sender, e) =>
                        {
                            shwSummary.Text = NCrr.Text;
                        };
                        LstCntrl = shwSummary;
                        allwaysShown.Controls.Add(LstCntrl);

                        LstCntrl = allwaysShown;
                        Hrzntl.Controls.Add(LstCntrl);
                    }//AlwaysShownDSGN
                   
                    {//HrzBottom
                        Hrzntl.Controls.Add(Sttus);
                        Height += Sttus.Height;
                        Sttus.FFWD.ClickOnly += (sender, e) =>
                        {
                            ShwOneCrrncsPosition = 0;
                            RstUserDates();
                            ShowOneCrrncy(CrrncsRates);
                        }; // ffwd click
                        Sttus.FWD.ClickOnly += (sender, e) =>
                        {
                            ShwOneCrrncsPosition--;
                            if (ShwOneCrrncsPosition < 0)
                            {
                                ShwOneCrrncsPosition = 0;
                            }
                            RstUserDates();
                            ShowOneCrrncy(CrrncsRates);
                        }; // fwd click
                        Sttus.BCK.ClickOnly += (sender, e) =>
                        {
                            ShwOneCrrncsPosition++;
                            if (ShwOneCrrncsPosition > CrrncsRates.Count - 1)
                            {
                                ShwOneCrrncsPosition = CrrncsRates.Count - 1;
                            }
                            RstUserDates();
                            ShowOneCrrncy(CrrncsRates);
                        }; // back click
                        Sttus.REW.ClickOnly += (sender, e) =>
                        {
                            ShwOneCrrncsPosition = CrrncsRates.Count - 1;
                            RstUserDates();
                            ShowOneCrrncy(CrrncsRates);
                            //Width += 10;
                        }; // rewind click

                        Sttus.StrtDate.Text = BttStrtDXm;// StrtDate
                        Sttus.StrtDate.ClickOnly += (sender, e) =>
                        {
                            //Sttus.StrtDate.Text = "zcatek";
                            {
                                DateTimePicker dtp = new DateTimePicker() { Dock = DockStyle.Fill, Size = smtimsShown.Size, Location = new Point(0, 0) };
                                Controls.Add(dtp);
                                dtp.Select();
                                SendKeys.Send("%{DOWN}");
                                dtp.ValueChanged += (senderf, f) => 
                                {

                                    {
                                        int ryear = dtp.Value.Year;
                                        int rmonth = dtp.Value.Month;
                                        int rday = dtp.Value.Day;
                                        int rhour = 0;
                                        int rminute = 0;
                                        int rsecond = 0;
                                        int rmiliSc = 0;
                                        SrStart_date = new DateTime(ryear, rmonth, rday, rhour, rminute, rsecond, rmiliSc);
                                        if (Ttle.Text == "BGN" && SrStart_date >= new DateTime(/*2004, 12, 10, 00, 00, 00*/2004, 5, 18, 00, 00, 00))
                                        {
                                            //18.5.2004

                                            RstSrStD();
                                            LblSrSDate.Text = LblStrtDXm0;//"Requested data are not available";
                                            //LblSrEDate.Text = "Requested data are not available";
                                            LblSrSDate.ForeColor = Color.SkyBlue;
                                            return;
                                        }//BGN
                                        else
                                        {
                                            if (SrStart_date > DateTime.Now.AddDays(-1))// Is status OK = LblStrtDXm2
                                            {
                                                ryear = DateTime.Now.Year;
                                                rmonth = DateTime.Now.Month;
                                                rday = DateTime.Now.AddDays(-1).Day;
                                                
                                                RstSrStD();
                                                SrStart_date = new DateTime(ryear, rmonth, rday, rhour, rminute, rsecond, rmiliSc);
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToString(/*"dd/MM/yyyy"*/) + " " + SrStart_date.DayOfWeek.ToString();// + "TEST/Error on:LblStrtDXm2";// + " is OK";
                                                LblSrSDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }// Is status OK = LblStrtDXm2
                                        }// Is status OK = LblStrtDXm2
                                        if (SrStart_date == SrEnd_date)
                                        {
                                            LblSrSDate.Text = LblStrtDXm1 + SrStart_date.ToString(/*"dd/MM/yyyy"*/) + " " + SrStart_date.DayOfWeek.ToString();// +"TEST/Error on:LblStrtDXm1";// + " is the same date as end day of interval";
                                            
                                            RstSrStD();
                                            LblSrSDate.ForeColor = Color.SkyBlue;
                                            LblSrSDate.Invalidate();
                                            return;
                                        }
                                        if (SrStart_date < SrEnd_date)
                                        {
                                            LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToString(/*"dd/MM/yyyy"*/) + " " + SrStart_date.DayOfWeek.ToString();

                                            SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            LblSrSDate.ForeColor = ClrdBck;
                                        }
                                        if (SrStart_date > SrEnd_date)
                                        {
                                            if (SrEnd_date!= new DateTime(1, 1, 0001, 00, 00, 00))
                                            {
                                                LblSrSDate.Text = LblStrtDXm3;// + "TEST/Error on:LblStrtDXm3";
                                                RstSrStD();
                                                LblSrSDate.ForeColor = Color.SkyBlue;
                                                return;
                                            }
                                            if (SrEnd_date == new DateTime(1, 1, 0001, 00, 00, 00))
                                            {
                                                LblSrSDate.Text = LblStrtDXm2 + SrStart_date.ToString(/*"dd/MM/yyyy"*/) + " " + SrStart_date.DayOfWeek.ToString();

                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                                LblSrSDate.ForeColor = ClrdBck;
                                            }
                                        }
                                        
                                        
                                    }

                                };
                                
                            }
                            
                        }; // StrtDate click

                        Sttus.EndDate.Text = BttEndDXm;// EndDate
                        Sttus.EndDate.ClickOnly += (sender, e) =>
                        {
                            {
                                DateTimePicker dtp = new DateTimePicker() { Dock = DockStyle.Fill, Size = smtimsShown.Size, Location = new Point(0, 0) };
                                //smtimsShown.Controls.Add(dtp);
                                Controls.Add(dtp);
                                dtp.Select();
                                //dtp.BringToFront();
                                SendKeys.Send("%{DOWN}");
                                dtp.ValueChanged += (senderf, f) => //{ };
                                {
                                    {
                                        int ryear = dtp.Value.Year;
                                        int rmonth = dtp.Value.Month;
                                        int rday = dtp.Value.Day;
                                        int rhour = 0;
                                        int rminute = 0;
                                        int rsecond = 0;
                                        int rmiliSc = 0;
                                        SrEnd_date = new DateTime(ryear, rmonth, rday, rhour, rminute, rsecond, rmiliSc);
                                        if (Ttle.Text == "BGN" && SrEnd_date >= new DateTime(/*2004, 12, 10, 00, 00, 00*/2004, 5, 18, 00, 00, 00))
                                        {
                                            //18.5.2004

                                            RstSrEnD();
                                            LblSrEDate.Text = LblStrtDXm0/*"Requested data are not available"*/;
                                            //LblSrEDate.Text = "Requested data are not available";
                                            LblSrEDate.ForeColor = Color.SkyBlue;
                                            return;
                                        }
                                        else
                                        {
                                            if (SrEnd_date>DateTime.Now)
                                            {
                                                RstSrEnD();
                                                SrEnd_date = DateTime.Now;
                                                LblSrEDate.Text = LblStrtDXm2 + SrEnd_date.ToString(/*"MM/dd/yyyy"*/) + " " + SrEnd_date.DayOfWeek.ToString();
                                                //LblSrEDate.Text = "Requested data are not available";
                                                //LblSrEDate.Text = "Requested data are not available";
                                                LblSrEDate.ForeColor = ClrdBck;
                                                SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            }
                                        }
                                        if (SrEnd_date == SrStart_date)
                                        {
                                            LblSrEDate.Text = LblStrtDXm1 + SrEnd_date.ToString("dd/MM/yyyy") + " " + SrEnd_date.DayOfWeek.ToString();
                                            RstSrEnD();
                                            LblSrEDate.ForeColor = Color.SkyBlue;
                                            return;
                                        }
                                        if (SrEnd_date < SrStart_date)
                                        {
                                            LblSrEDate.Text = LblStrtDXm3;
                                            RstSrEnD();
                                            //
                                            LblSrEDate.ForeColor = Color.SkyBlue;
                                        }
                                        if (SrEnd_date > SrStart_date)
                                        {
                                            LblSrEDate.Text = LblStrtDXm2+ SrEnd_date.ToString("MM/dd/yyyy") + " " + SrEnd_date.DayOfWeek.ToString();
                                            SniffIt(); ShowOneCrrncy(CrrncsRates);
                                            LblSrEDate.ForeColor = ClrdBck;
                                            //return;
                                        }//Status OK = LblStrtDXm2


                                    }

                                };
                                //ShowOneCrrncy(CrrncsRates);
                            }
                            
                        }; // EndDate click

                        Sttus.EUR.ClickOnly += (sender, e) =>
                        {
                            Phase++;
                        }; // EUR click
                        //Sttus.Items.Add(LnguageContextMenuStrip);
                    }//HrzBottom

                    
                    ClientSizeChanged += (sender, e) =>
                    {
                        if (Width > MSzz.Width && ShwNws == false)
                        {
                            {//btnholder
                                btnHldr.Height = allwaysShown.Height / 2;
                                btnHldr.Width = allwaysShown.Width / 3;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.BackColor = ClrdBck;
                                btnHldr.Dock = DockStyle.Left;
                                //
                                icn.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                icn.Invalidate();

                                //

                                LstDataRefresh.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                LstDataRefresh.Location = new Point(icn.Right, 0); LstDataRefresh.Anchor = AnchorStyles.Top;
                                LstDataRefresh.Invalidate();
                                btnHldr.Invalidate();
                            }//btnholder
                        }
                        if (Height > MSzz.Height && ShwNws == false)
                        {
                            {//btnHldr
                                btnHldr.Height = allwaysShown.Height;
                                btnHldr.Width = allwaysShown.Width / DsgnModifierWdth;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.Dock = DockStyle.Left;

                                icn.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btnHldr.ClientSizeChanged += (sendef, f) =>
                                {
                                    icn.Width = btnHldr.Width;
                                    icn.Height = btnHldr.Height / 2;
                                    icn.Location = new Point(0, 0);
                                };

                                LstDataRefresh.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                LstDataRefresh.Location = new Point(0, icn.Bottom);
                                LstDataRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                                btnHldr.ClientSizeChanged += (senderr, r) =>
                                {
                                    LstDataRefresh.Location = new Point(0, icn.Bottom);
                                    LstDataRefresh.Width = btnHldr.Width; LstDataRefresh.Height = btnHldr.Height / 2;
                                };
                            }//btnHldr

                        }

                        if (Width > MSzz.Width && ShwNws == true)
                        {
                            {//btnholder
                                btnHldr.Height = allwaysShown.Height / 2;
                                btnHldr.Width = allwaysShown.Width / 3;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.BackColor = ClrdBck;
                                btnHldr.Dock = DockStyle.Left;

                                //

                                icn.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                icn.Invalidate();

                                //

                                LstDataRefresh.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                LstDataRefresh.Location = new Point(icn.Right, 0);
                                LstDataRefresh.Anchor = AnchorStyles.Top;
                                LstDataRefresh.Invalidate();
                                btnHldr.Invalidate();
                            }//btnholder
                        }
                        else
                        {
                            {//btnHldr
                                btnHldr.Height = allwaysShown.Height;
                                btnHldr.Width = allwaysShown.Width / DsgnModifierWdth;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.Dock = DockStyle.Left;

                                icn.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btnHldr.ClientSizeChanged += (sendef, f) =>
                                {
                                    icn.Width = btnHldr.Width;
                                    icn.Height = btnHldr.Height / 2;
                                    icn.Location = new Point(0, 0);
                                };

                                LstDataRefresh.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                LstDataRefresh.Location = new Point(0, icn.Bottom);
                                LstDataRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                                btnHldr.ClientSizeChanged += (senderr, r) =>
                                {
                                    LstDataRefresh.Location = new Point(0, icn.Bottom);
                                    LstDataRefresh.Width = btnHldr.Width; LstDataRefresh.Height = btnHldr.Height / 2;
                                };
                            }//btnHldr
                        }
                        if (Height > MSzz.Height * 3 && ShwNws == true)
                        {
                            {//btnHldr
                                btnHldr.Height = allwaysShown.Height;
                                btnHldr.Width = allwaysShown.Width / DsgnModifierWdth;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.Dock = DockStyle.Left;

                                icn.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                btnHldr.ClientSizeChanged += (sendef, f) =>
                                {
                                    icn.Width = btnHldr.Width;
                                    icn.Height = btnHldr.Height / 2;
                                    icn.Location = new Point(0, 0);
                                };

                                LstDataRefresh.Size = new Size(btnHldr.Width, btnHldr.Height / 2);
                                LstDataRefresh.Location = new Point(0, icn.Bottom);
                                LstDataRefresh.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                                btnHldr.ClientSizeChanged += (senderr, r) =>
                                {
                                    LstDataRefresh.Location = new Point(0, icn.Bottom);
                                    LstDataRefresh.Width = btnHldr.Width; LstDataRefresh.Height = btnHldr.Height / 2;
                                };
                            }//btnHldr

                        }
                        else
                        {
                            {//btnholder
                                btnHldr.Height = allwaysShown.Height / 2;
                                btnHldr.Width = allwaysShown.Width / 3;
                                btnHldr.Location = new Point(0, 0);
                                btnHldr.BackColor = ClrdBck;
                                btnHldr.Dock = DockStyle.Left;

                                //

                                icn.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                icn.Location = new Point(0, 0);
                                icn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                                icn.Invalidate();

                                //

                                LstDataRefresh.Size = new Size(btnHldr.Width / 2, btnHldr.Height);
                                LstDataRefresh.Location = new Point(icn.Right, 0);
                                LstDataRefresh.Anchor = AnchorStyles.Top;
                                LstDataRefresh.Invalidate();
                                btnHldr.Invalidate();
                            }//btnholder
                        }
                        DrawGraph();
                        Invalidate();
                        foreach (Control item in Controls)
                        {
                            item.Invalidate();
                        }

                    };
                    
                }//DSG
                foreach (Control c in Hrzntl.Controls)
                {
                    //if (c.Text!="")
                    //{
                    //    c.MouseHover += (sender, e) =>
                    //    {
                    //        ToolTip hnt = new ToolTip();
                    //        hnt.SetToolTip(c, c.Text);
                    //    };
                    //}
                    //if (c.Controls.Count>0)
                    //{
                    //    foreach (Control cc in c.Controls)
                    //    {
                    //        if (cc.Text != "")
                    //        {
                    //            cc.MouseHover += (sender, e) =>
                    //            {
                    //                ToolTip hnt = new ToolTip();
                    //                hnt.SetToolTip(cc, cc.Text);
                    //            };
                    //        }
                    //        if (cc.Controls.Count > 0)
                    //        {
                    //            foreach (Control ccc in cc.Controls)
                    //            {
                    //                if (ccc.Text != "")
                    //                {
                    //                    ccc.MouseHover += (sender, e) =>
                    //                    {
                    //                        ToolTip hnt = new ToolTip();
                    //                        hnt.SetToolTip(ccc, ccc.Text);
                    //                    };
                    //                }
                    //                if (ccc.Controls.Count > 0)
                    //                {
                    //                    foreach (Control cccc in ccc.Controls)
                    //                    {
                    //                        if (cccc.Text != "")
                    //                        {
                    //                            cccc.MouseHover += (sender, e) =>
                    //                            {
                    //                                ToolTip hnt = new ToolTip();
                    //                                hnt.SetToolTip(cccc, cccc.Text);
                    //                            };
                    //                        }
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
                Prgrm.Controls.Add(Hrzntl);
                {//LngButtonDsg
                    {
                        Button lng = new Button() { Dock = DockStyle.Top, Height = 22,Width=Hrzntl.Width,Location=new Point(0,0),BackColor=ClrdBck,ForeColor=ClrdFrnt,Text=BttLngXm,TextAlign=ContentAlignment.MiddleCenter };
                        lng.Click += (sender, e) => 
                        {
                            try
                            {
                                nwFrm.Close();
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                            nwFrm = new Form() { FormBorderStyle = FormBorderStyle.None, Location= PointToScreen(MousePosition), Size=new Size(MSzz.Width/3, 3*24),BackColor=ClrdFrnt,ForeColor=ClrdFrnt};
                            
                            nwFrm.Click += (senderf, f) =>
                            {
                                nwFrm.Close();
                            };
                            nwFrm.BringToFront();
                            nwFrm.Show();
                            
                            {//nwFrmDSG
                                {
                                    Panel dsg = new Panel() { Size=new Size(nwFrm.Width-6, nwFrm.Height-6),Location=new Point(3,3),BackColor=ClrdFrnt };
                                    Panel btnHldr_nwFrm = new Panel();
                                    btnHldr_nwFrm.Height = dsg.Height;
                                    btnHldr_nwFrm.Width = dsg.Width;
                                    btnHldr_nwFrm.Location = new Point(0, 0);
                                    btnHldr_nwFrm.BackColor = Color.Transparent;
                                    btnHldr_nwFrm.Dock = DockStyle.Fill;
                                    //btnHldr_nwFrm.BorderStyle = BorderStyle.Fixed3D;
                                    Label cz = new Label () { TextAlign=ContentAlignment.MiddleCenter,BackColor=ClrdBck,ForeColor=ClrdFrnt,Size = new Size(btnHldr_nwFrm.Width, btnHldr_nwFrm.Height/3),Location=new Point(1,1),Dock=DockStyle.Top, Text = "Čeština",Font=ContentFont };

                                    //btnHldr_nwFrm.ClientSizeChanged += (senderh, h) => { cz.Width = btnHldr_nwFrm.Width/2-3; cz.Height = btnHldr_nwFrm.Height-2; };
                                    
                                    cz.Font = ContentFont;
                                    cz.Click += (senderh, h) =>
                                    {
                                        LngSetting = 0;
                                        foreach (Control c in Controls)
                                        {
                                            //c.Invalidate();
                                            //if (c.Controls.Count>0)
                                            //{
                                            //    foreach (Control cc in c.Controls)
                                            //    {
                                            //        cc.Invalidate();
                                            //        if (cc.Controls.Count > 0)
                                            //        {
                                            //            foreach (Control ccc in cc.Controls)
                                            //            {
                                            //                ccc.Invalidate();
                                            //                if (ccc.Controls.Count > 0)
                                            //                {
                                            //                    foreach (Control cccc in ccc.Controls)
                                            //                    {
                                            //                        cccc.Invalidate();
                                            //                        if (cccc.Controls.Count > 0)
                                            //                        {
                                            //                            foreach (Control ccccc in Controls)
                                            //                            {
                                            //                                ccccc.Invalidate();
                                                                            
                                            //                            }
                                            //                        }
                                            //                    }
                                            //                }
                                            //            }
                                            //        }
                                            //    }
                                            //}
                                        }
                                        nwFrm.Close();
                                        Application.Restart();
                                    };
                                    cz.MouseHover += (senderh, h) =>
                                    {
                                        ToolTip hnt = new ToolTip();
                                        hnt.AutomaticDelay = 100;
                                        hnt.AutoPopDelay = 3000;
                                        hnt.ReshowDelay = 3100;
                                        hnt.SetToolTip(cz, "Nastavte češtinu jako výchozí jazyk aplikace"+Environment.NewLine+"Po kliknutí bude program restartován s novým nastavením.");
                                    };
                                    btnHldr_nwFrm.Controls.Add(cz);

                                    Label clse = new Label() { TextAlign = ContentAlignment.MiddleCenter, BackColor = ClrdBck, ForeColor = ClrdFrnt, Size = new Size(btnHldr_nwFrm.Width , btnHldr_nwFrm.Height /3), Location = new Point(0, cz.Bottom), Dock = DockStyle.Bottom, Text = "Cancel", Font = ContentFont };

                                    //btnHldr_nwFrm.ClientSizeChanged += (senderh, h) => { cz.Width = btnHldr_nwFrm.Width/2-3; cz.Height = btnHldr_nwFrm.Height-2; };

                                    clse.Font = ContentFont;
                                    clse.Click += (senderh, h) =>
                                    {
                                        nwFrm.Close();
                                    };
                                    clse.MouseHover += (senderh, h) =>
                                    {
                                        
                                        //hnt.SetToolTip(cz, "Nastavte češtinu jako výchozí jazyk aplikace" + Environment.NewLine + "Po kliknutí bude program restartován s novým nastavením.");
                                    };
                                    btnHldr_nwFrm.Controls.Add(clse);


                                    Label eng = new Label(){ Location = new Point(0, cz.Bottom), TextAlign = ContentAlignment.MiddleCenter,Font = ContentFont,ForeColor = ClrdFrnt,BackColor = ClrdBck, Size = new Size(btnHldr_nwFrm.Width, btnHldr_nwFrm.Height / 3), Dock = DockStyle.Top, Text = "English" };
                                    
                                    //eng.Anchor = AnchorStyles.Right-eng.Width | AnchorStyles.Top;
                                    //btnHldr_nwFrm.ClientSizeChanged += (senderh, h) => { eng.Width = btnHldr_nwFrm.Width / 2; eng.Height = btnHldr_nwFrm.Height - 22; };

                                    eng.MouseHover += (senderh, h) =>
                                    {
                                        ToolTip hnt = new ToolTip();
                                        hnt.AutomaticDelay = 100;
                                        hnt.AutoPopDelay = 3000;
                                        hnt.ReshowDelay = 3100;
                                        hnt.SetToolTip(eng, "Set english as the main language of application"+Environment.NewLine+ "The application will restart in ordrer to take new changes");
                                    };
                                    eng.Click += (senderh, h) =>
                                    {
                                        LngSetting = 1;
                                        //CultureInfo CultureInfoUs = new CultureInfo("en-us");

                                        nwFrm.Close();
                                        Application.Restart();
                                    };
                                    btnHldr_nwFrm.Controls.Add(eng);
                                    //Label cls = new Label() { Size = new Size(btnHldr_nwFrm.Width - 2, 22), Location = new Point(1, btnHldr_nwFrm.Bottom-Height), Text = "Close" };
                                    //cls.Click += (senderh, h) => { nwFrm.Close(); };
                                    //btnHldr_nwFrm.Controls.Add(cls);
                                    dsg.Controls.Add(btnHldr_nwFrm);
                                    
                                    LstCntrl = dsg;
                                    nwFrm.Controls.Add(LstCntrl);
                                }//btnholder
                            }//nwFrmDSG
                            nwFrm.SizeChanged += (senderf, f) =>
                            {
                                //cz.Width = Prgrm.Width / 2;
                                //eng.Width = Prgrm.Width / 2;
                            };

                            //nwFrm.Controls.Add(cz);
                            //nwFrm.Controls.Add(eng);
                            //nwFrm.Controls.Add(Hrzntl);
                        };
                        Prgrm.Controls.Add(lng);
                        Height += lng.Height;
                    }
                }//LngButtonDsg
                MSzz = new Size(Width, (Height));// + S.F.STATUS.Height);
                foreach (Control c in Hrzntl.Controls)
                {

                    c.KeyDown += (sender, e) =>
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.PageUp:
                                FontSize = FontSize + 2;
                                break;
                            case Keys.PageDown:
                                FontSize = FontSize - 2;
                                break;
                            default:
                                break;
                        }
                    };
                    if (c.Controls.Count > 0)
                    {
                        foreach (Control cc in c.Controls)
                        {
                            cc.KeyDown += (sender, e) =>
                            {
                                switch (e.KeyCode)
                                {
                                    case Keys.PageUp:
                                        FontSize = FontSize + 2;
                                        break;
                                    case Keys.PageDown:
                                        FontSize = FontSize - 2;
                                        break;
                                    default:
                                        break;
                                }
                            };
                            if (c.Controls.Count > 0)
                            {
                                foreach (Control ccc in cc.Controls)
                                {
                                    ccc.KeyDown += (sender, e) =>
                                    {
                                        switch (e.KeyCode)
                                        {
                                            case Keys.PageUp:
                                                FontSize = FontSize + 2;
                                                break;
                                            case Keys.PageDown:
                                                FontSize = FontSize - 2;
                                                break;
                                            default:
                                                break;
                                        }
                                    };
                                }
                            }
                        }
                    }
                }
            }//Horizontal Panel -Hrzntl



            ClientSizeChanged += (sender, e) =>
            {
                Prgrm.Invalidate();
            };

        }//DSGN
        private void RstUserDates()
        {
            SrStart_date = new DateTime(1, 1, 0001, 00, 00, 00);
            SrEnd_date = new DateTime(1, 1, 0001, 00, 00, 00);
            LblSrSDate.Text = "";
            LblSrEDate.Text = "";
        }
        private void RstSrEnD()
        {
            SrEnd_date = new DateTime(1, 1, 0001, 00, 00, 00);
            //SrEnd_date = new DateTime(1, 1, 0001, 00, 00, 00);
            //LblSrSDate.Text = "";
            //LblSrEDate.Text = "";
        }
        private void RstSrStD()
        {
            SrStart_date = new DateTime(1, 1, 0001, 00, 00, 00);
            //SrEnd_date = new DateTime(1, 1, 0001, 00, 00, 00);
            //LblSrSDate.Text = "";
            //LblSrEDate.Text = "";
        }//FUNCTION - private----

        //FUNCTION PUBLIC----------------------------------------------  
        public void MyClrInvrt(Color cl)
        {
            ClrInver = Color.FromArgb(cl.ToArgb() ^ 0xffffff);
            ForeColor = ClrInver;
        }
        public void DbgTltps()
        {//Position of added controls

            {//Draw over Time
                {//Create new Draw over Time for later execution
                 //MTltip.DrwOverTime drwot = new MTltip.DrwOverTime(50);

                    //PrsmSlct.ReceiveMove(prsmSlctUsrVal, 43);
                }//Create new Draw over Time for later execution
                {//asign whoo will be drawed
                }//asign whoo will be drawed
                {//Execute && DRAW Move3d
                    Cnst.S.Ticker.Interval = 33;

                    Cnst.S.Ticker.Tick += (senderf, f) =>
                    {
                        //foreach (MTltip q in Cnst.S.TPaint)
                        //{
                        //    q.Ticker_Tick2();
                        //    //q.Ticker_Tick();

                        //}
                        //Cnst.S.TPaint.RemoveAll(q => q.isDrwing == false);
                        //if (Cnst.S.TPaint.Count == 0) Cnst.S.Ticker.Stop();

                        ////S.Cnvs.Refresh();

                    };
                }//Execute && DRAW Move3d
            }



            Invalidate();
            //MessageBox.Show(Cnst.Source.ClientRectangle.Size.ToString());

            foreach (Control c in Controls)
            {

                c.ForeColor = Color.FromArgb(32, 78, 70);
                c.Invalidate();
                {
                }
                if (c.Controls.Count > 0)
                {
                    foreach (Control Cc in Controls)
                    {
                        Cc.ForeColor = Color.FromArgb(32, 78, 70);
                        Cc.Invalidate();
                        if (Cc.Controls.Count > 0)
                        {
                            foreach (Control Ccc in Controls)
                            {
                                Ccc.ForeColor = Color.FromArgb(32, 78, 70);
                                Ccc.Invalidate();
                            }
                        }
                    }

                }
            }
            //Controls.Clear();
            //HdrDsgn();
        }
        public void RposCntrols()
        {//Position of added controls
            Invalidate();
            
            foreach (Control c in Controls)
            {
                //c.ForeColor = Color.FromArgb(32, 78, 70);
                c.Invalidate();
                if (c.Controls.Count > 0)
                {
                    foreach (Control Cc in Controls)
                    {
                        //Cc.ForeColor = Color.FromArgb(32, 78, 70);
                        Cc.Invalidate();
                        if (Cc.Controls.Count > 0)
                        {
                            foreach (Control Ccc in Controls)
                            {
                                //Ccc.ForeColor = Color.FromArgb(32, 78, 70);
                                Ccc.Invalidate();
                            }
                        }
                    }

                }
            }

        }
        public void PrintScreen()
        {
            //!!SAVE PICTURE FILE ON DISC!!---------------------------------
            Bitmap bmp = new Bitmap(this.Width, this.Height);
            this.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
            string myPath = Application.StartupPath;//Setting HomeFolder for saving future PrtScr.jpg
            myPath += "\\SpliterScreen.jpg"; //Adding Name for PrtScr.jpg
            try
            {
                bmp.Save(myPath, ImageFormat.Jpeg);//SAVE!!
            }
            catch (Exception) { }
            //!!SAVE PICTURE FILE ON DISC!!---------------------------------
            {
                //PrintDocument pd = new PrintDocument();
                //pd.DefaultPageSettings.PrinterSettings.PrinterName = "Printer Name";
                //pd.DefaultPageSettings.Landscape = true; //or false!
                //pd.PrintPage += (sender, args) =>
                //{
                //    Image i = Image.FromFile(myPath += "\\SpliterScreen.jpg");
                //    Rectangle m = args.MarginBounds;

                //    if ((double)i.Width / (double)i.Height > (double)m.Width / (double)m.Height) // image is wider
                //    {
                //        m.Height = (int)((double)i.Height / (double)i.Width * (double)m.Width);
                //    }
                //    else
                //    {
                //        m.Width = (int)((double)i.Width / (double)i.Height * (double)m.Height);
                //    }
                //    args.Graphics.DrawImage(i, m);
                //};
                //pd.Print();

            }
            //PdfDocument doc = new PdfDocument();
            //doc.Pages.Add(new PdfPage());
            //XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
            //XImage img = XImage.FromFile(source);
            //xgr.DrawImage(img, 0, 0);
            //doc.Save(myPath += "\\SpliterScreen.pdf"); doc.Close();
        }//FUNCTION PUBLIC----------------------------------------------  


    }

    //Classes

    

    //context menu for Textures

    //context menu for Panels




}
