using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace HarFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private BindingSource source = new BindingSource();
        private BindingSource source2 = new BindingSource();
        private BindingSource source3 = new BindingSource();
        private BindingSource source4 = new BindingSource();
        List<HarFile.Outlier> outliers = new List<HarFile.Outlier>();
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofdBrowse.ShowDialog();
            txtFilePath.Text = ofdBrowse.FileName;
        }

        private void btnProcessFile_Click(object sender, EventArgs e)
        {
            string result = "";
            HarFile harfile;
            try
            {
               using (StreamReader sr = new StreamReader(txtFilePath.Text))
                {
                    result = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem accessing file");
                return;
            }
            try
            {
                harfile = JsonConvert.DeserializeObject<HarFile>(result);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Problem processing file");
                return;
            }
            // process the file and build collections of Stat objects. Each stat is a summary of data for a unique item (URL/file type)

            List<HarFile.Stat> urlStats = new List<HarFile.Stat>();
            List<HarFile.Stat> typeStats = new List<HarFile.Stat>();
            List<HarFile.Error> errors = new List<HarFile.Error>();
            

            foreach (HarFile.Entry entry in harfile.log.entries)
            {
                
                string url = entry.request.url;
                string type;
                //remove URL parameters
                if (url.Contains("?")) url = url.Substring(0, url.IndexOf("?"));
                if (url.LastIndexOf('/') > url.LastIndexOf('.'))
                {
                    type = url.Substring(url.LastIndexOf('/') + 1).ToLower();
                }
                else type = url.Substring(url.LastIndexOf('.') + 1).ToLower();
                urlStats = processStat(urlStats, entry, url);
                typeStats = processStat(typeStats,entry,type);
                // find errors, create error object, add to list
                if (entry.response.status > 399)
                {
                    HarFile.Error errorResp = new HarFile.Error();
                    errorResp.timestamp = entry.startedDateTime.ToString();
                    errorResp.time = entry.time;
                    errorResp.url = url;
                    errorResp.queryString = entry.request.queryString;
                    errorResp.status = entry.response.status;
                    errorResp.responseHeaders = entry.response.headers;
                    errorResp.errorText = entry.response.content.text;
                    errors.Add(errorResp);
                }
                
            }
            urlStats = calcAverages(urlStats, "url");
            typeStats = calcAverages(typeStats, "type");
            
            //display stats
            dgvStats.DataSource = null;
            source.DataSource = urlStats;
            dgvStats.DataSource = source;
            dgvStats.Refresh();

            dgvType.DataSource = null;
            source2.DataSource = typeStats;
            dgvType.DataSource = source2;
            dgvType.Refresh();

            dgvErrors.DataSource = null;
            source3.DataSource = errors;
            dgvErrors.DataSource = source3;
            dgvErrors.Refresh();

            dgvOutliers.DataSource = null;
            source4.DataSource = outliers;
            dgvOutliers.DataSource = source4;
            dgvOutliers.Refresh();

        }

        private List<HarFile.Stat> calcAverages(List<HarFile.Stat> statlist, string type)
        // create the averages from accumulated totals
        {
            
            foreach (HarFile.Stat stat in statlist)
            {
                stat.avgBlocked = stat.avgBlocked / stat.count;
                stat.avgConnect = stat.avgConnect / stat.count;
                stat.avgDNS = stat.avgDNS / stat.count;
                stat.avgDownload = stat.avgDownload / stat.count;
                stat.avgLoad = stat.avgLoad / stat.count;
                stat.avgSend = stat.avgSend / stat.count;
                stat.avgSSL = stat.avgSSL / stat.count;
                stat.avgWait = stat.avgWait / stat.count;
                double varNum = 0;
                foreach (HarFile.SimpleEntry entry in stat.entries)
                {
                    varNum += Math.Pow(stat.avgLoad - entry.time, 2);
                }
                double sd = Math.Sqrt(varNum / stat.entries.Count);
                foreach (HarFile.SimpleEntry entry in stat.entries)
                {
                    if (entry.time > stat.avgLoad + (3 * sd)) //outlier
                    {
                        HarFile.Outlier ol = new HarFile.Outlier();
                        ol.avgTime = stat.avgLoad;
                        ol.responseSize = entry.responseSize;
                        ol.sd = (float) sd;
                        ol.sdCount = (int) Math.Round((entry.time - stat.avgLoad) / sd);
                        ol.time = entry.time;
                        ol.timestamp = entry.timestamp;
                        ol.type = type;
                        ol.url = entry.url;
                        outliers.Add(ol);
                    }
                }

            }
            return statlist;
        }
        private List<HarFile.Stat> processStat(List<HarFile.Stat> statlist, HarFile.Entry entry, string item)
        {
            int code = entry.response.status / 100;
            HarFile.SimpleEntry se = new HarFile.SimpleEntry();
            se.responseSize = entry.response.content.size;
            se.time = entry.time;
            se.timestamp = entry.startedDateTime.ToString();
            se.url = entry.request.url;
            //find existing stat
            if (statlist.Where(x => x.item == item).Count() > 0)
            {
                HarFile.Stat match = statlist.Where(x => x.item == item).ElementAt(0);
                statlist.RemoveAll(x => x.item == item);
                match.count++;
                switch (code)
                {
                    case 2:
                        match.count200++;
                        break;
                    case 4:
                        match.count400++;
                        break;
                    case 5:
                        match.count500++;
                        break;
                    default:
                        break;
                }
                match.entries.Add(se);
                match.avgBlocked += Math.Max(entry.timings.blocked, 0);
                match.longestBlocked = Math.Max(match.longestBlocked, entry.timings.blocked);
                match.avgConnect += Math.Max(entry.timings.connect, 0);
                match.longestConnect = Math.Max(match.longestConnect, entry.timings.connect);
                match.avgDNS += Math.Max(entry.timings.dns, 0);
                match.longestDNS = Math.Max(match.longestDNS, entry.timings.dns);
                match.avgDownload += Math.Max(entry.timings.receive, 0);
                match.longestDownload = Math.Max(match.longestDownload, entry.timings.receive);
                match.avgLoad += Math.Max(entry.time, 0);
                match.longestLoad = Math.Max(match.longestLoad, entry.time);
                match.avgSend += Math.Max(entry.timings.send, 0);
                match.longestSend = Math.Max(match.longestSend, entry.timings.send);
                match.avgSSL += Math.Max(entry.timings.ssl, 0);
                match.longestSSL = Math.Max(match.longestSSL, entry.timings.ssl);
                match.avgWait += Math.Max(entry.timings.wait, 0);
                match.longestWait = Math.Max(match.longestWait, entry.timings.wait);
                statlist.Add(match);
                return statlist;
            }
            HarFile.Stat newStat = new HarFile.Stat();
            newStat.entries = new List<HarFile.SimpleEntry>();
            newStat.item = item;
            newStat.count = 1;

            switch (code)
            {
                case 2:
                    newStat.count200 = 1;
                    newStat.count400 = 0;
                    newStat.count500 = 0;
                    break;
                case 4:
                    newStat.count200 = 0;
                    newStat.count400 = 1;
                    newStat.count500 = 0;
                    break;
                case 5:
                    newStat.count200 = 0;
                    newStat.count400 = 0;
                    newStat.count500 = 1;
                    break;
                default:
                    break;
            }
            newStat.entries.Add(se);
            newStat.avgLoad = Math.Max(entry.time, 0);
            newStat.longestLoad = Math.Max(entry.time, 0);
            newStat.avgBlocked = Math.Max(entry.timings.blocked, 0);
            newStat.longestBlocked = Math.Max(entry.timings.blocked, 0);
            newStat.avgDNS = Math.Max(entry.timings.dns, 0);
            newStat.longestDNS = Math.Max(entry.timings.dns, 0);
            newStat.avgConnect = Math.Max(entry.timings.connect, 0);
            newStat.longestConnect = Math.Max(entry.timings.connect, 0);
            newStat.avgSSL = Math.Max(entry.timings.ssl, 0);
            newStat.longestSSL = Math.Max(entry.timings.ssl, 0);
            newStat.avgSend = Math.Max(entry.timings.send, 0);
            newStat.longestSend = Math.Max(entry.timings.send, 0);
            newStat.avgWait = Math.Max(entry.timings.wait, 0);
            newStat.longestWait = Math.Max(entry.timings.wait, 0);
            newStat.avgDownload = Math.Max(entry.timings.receive, 0);
            newStat.longestDownload = Math.Max(entry.timings.receive, 0);
            statlist.Add(newStat);
            return statlist;
        }
    }
}
