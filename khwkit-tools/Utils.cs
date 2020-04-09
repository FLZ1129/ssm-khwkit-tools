using CrazySharp.Base;
using CrazySharp.Base.Attributes;
using CrazySharp.Base.Enums;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazySharp.Std
{
    public static class Utils
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static string SN()
        {
            var phyNics = NetworkInterface.GetAllNetworkInterfaces().Where(
                     it => it.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                     &&
                     !(it.Description.ToLower().Contains("vmware") ||
                     it.Description.ToLower().Contains("vitual") ||
                     it.Description.ToLower().Contains("bluetooth") ||
                     it.Description.ToLower().Contains("tap-windows")
                     ));
            string tmp = phyNics.FirstOrDefault()?.GetPhysicalAddress().ToString() ?? "";
            char[] charArray = tmp.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        public static string LocalIp()
        {
            var phyNics = NetworkInterface.GetAllNetworkInterfaces().Where(
                     it => it.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                     &&
                     !(it.Description.ToLower().Contains("vmware") ||
                     it.Description.ToLower().Contains("vitual") ||
                     it.Description.ToLower().Contains("bluetooth") ||
                     it.Description.ToLower().Contains("tap-windows")
                     ));
            return phyNics.FirstOrDefault()
                ?.GetIPProperties()
                ?.UnicastAddresses
                ?.FirstOrDefault(it => it.Address.AddressFamily == AddressFamily.InterNetwork)
                ?.Address.ToString() ?? ""; ;
        }
        /// <summary>
        /// 在锁的范围内,进行Action的调用
        /// </summary>
        /// <param name="mtx"></param>
        /// <param name="a"></param>
        public static void MutexOperation(Mutex mtx, Action a)
        {
            try
            {
                mtx.WaitOne();
                a.Invoke();
            }
            finally
            {
                mtx.ReleaseMutex();
            }
        }

        /// <summary>
        /// 并行遍历数据,同时返回所有数据处理的结果
        /// </summary>
        /// <typeparam name="TRet"></typeparam>
        /// <typeparam name="TEle"></typeparam>
        /// <param name="ls"></param>
        /// <param name="mapFn"></param>
        /// <returns></returns>
        public static List<TRet> ParallelMap<TRet, TEle>(IEnumerable<TEle> ls, Func<TEle, TRet> mapFn)
        {
            var enumerable = ls as TEle[] ?? ls.ToArray();
            List<TRet> ret = new List<TRet>(enumerable.Count());
            using (Mutex mtx = new Mutex())
            {
                Parallel.ForEach(enumerable, (e) =>
                {
                    var funRet = mapFn(e);
                    if (funRet != null)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        mtx.WaitOne();
                        ret.Add(funRet);
                        // ReSharper disable once AccessToDisposedClosure
                        mtx.ReleaseMutex();
                    }
                });
            }
            return ret;
        }

        /// <summary>
        /// 生成UUID,默认format为"N"
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public static string GenUUID(string format = "N")
        {
            return Guid.NewGuid().ToString(format);
        }

        /// <summary>
        /// 弹出错误窗口
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="buttons">默认Yes</param>
        /// <returns></returns>
        public static DialogResult Error(string msg,
            string title = "系统提示",
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, title, buttons, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 弹出警告窗口
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="buttons">默认Yes</param>
        /// <returns></returns>
        public static DialogResult Warn(string msg,
            string title = "系统提示",
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, title, buttons, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// 弹出疑问窗口
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="buttons">默认YesNo</param>
        /// <returns></returns>
        public static DialogResult Ask(string msg,
            string title = "系统提示",
            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel)
        {
            return MessageBox.Show(msg, title, buttons, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 弹出信息窗口
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static DialogResult Info(string msg,
            string title = "系统提示",
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(msg, title, buttons, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 判断字符串是否为空,null or len=0 返回true
        /// </summary>
        /// <param name="str"></param>
        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        public static bool IsNotEmpty(string str)
        {
            return !IsEmpty(str);
        }

        /// <summary>
        /// 判断集合是否为空,null or count=0 返回true
        /// </summary>
        /// <param name="arr"></param>
        public static bool IsEmpty(ICollection arr)
        {
            return arr == null || arr.Count == 0;
        }

        private static string GetFilterStr(FileTypeFilter filter)
        {
            var filterList = new List<string>();
            if ((filter & FileTypeFilter.BIN) == FileTypeFilter.BIN)
            {
                filterList.Add("Binary Files(*.bin)|*.bin");
            }
            if ((filter & FileTypeFilter.PBIN) == FileTypeFilter.PBIN)
            {
                filterList.Add("Puietel Binary Files(*.pbin)|*.pbin");
            }
            if ((filter & FileTypeFilter.PSDATA) == FileTypeFilter.PSDATA)
            {
                filterList.Add("Puietel Suite Data Files(*.psdata)|*.psdata");
            }
            if ((filter & FileTypeFilter.PPBAK) == FileTypeFilter.PPBAK)
            {
                filterList.Add("Puietel Project Backup Files(*.ppbak)|*.ppbak");
            }
            if ((filter & FileTypeFilter.TXT) == FileTypeFilter.TXT)
            {
                filterList.Add("Text Files(*.txt)|*.txt");
            }

            if ((filter & FileTypeFilter.JSON) == FileTypeFilter.JSON)
            {
                filterList.Add("Json Files(*.json)|*.json");
            }

            if ((filter & FileTypeFilter.EXCEL) == FileTypeFilter.EXCEL)
            {
                filterList.Add("Excel Files(*.xls,*.xlsx)|*.xls;*.xlsx");
            }

            if ((filter & FileTypeFilter.WORD) == FileTypeFilter.WORD)
            {
                filterList.Add("Document Files(*.doc,*.docx)|*.doc;*.docx");
            }

            if ((filter & FileTypeFilter.PPT) == FileTypeFilter.PPT)
            {
                filterList.Add("PowerPoint Files(*.ppt,*.pptx)|*.ppt;*.pptx");
            }

            if ((filter & FileTypeFilter.LICENSE) == FileTypeFilter.LICENSE)
            {
                filterList.Add("License Files(*.license)|*.license");
            }
            if ((filter & FileTypeFilter.IMAGE) == FileTypeFilter.IMAGE)
            {
                filterList.Add("Image Files(*.bmp;*.jpg;*.gif)|*.bmp;*.jpg;*.gif");
            }

            filterList.Add("All files (*.*)|*.*");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filterList.Count() - 1; ++i)
            {
                sb.AppendFormat("{0}|", filterList[i]);
            }

            sb.Append(filterList[filterList.Count() - 1]);
            return sb.ToString();
        }

        /// <summary>
        /// 打开多个文件
        /// </summary>
        /// <param name="filesOk"></param>
        public static void OpenFile(Action<List<string>> filesOk)
        {
            OpenFile(FileTypeFilter.ALL, filesOk);
        }

        /// <summary>
        /// 打开多个文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filesOk"></param>
        public static void OpenFile(FileTypeFilter filter, Action<List<string>> filesOk)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = GetFilterStr(filter),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = true,
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var fileNames = dialog.FileNames;
                if (fileNames == null || fileNames.Length == 0)
                {
                    return;
                }

                filesOk?.Invoke(fileNames.ToList());
            }
        }

        /// <summary>
        /// 打开一个文件
        /// </summary>
        /// <param name="fileOk"></param>
        public static void OpenFile(Action<string> fileOk)
        {
            OpenFile(FileTypeFilter.ALL, fileOk);
        }

        /// <summary>
        /// 打开一个文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fileOk"></param>
        public static void OpenFile(FileTypeFilter filter, Action<string> fileOk)
        {
            Debug.Assert(fileOk != null);
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = GetFilterStr(filter),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                CheckFileExists = true,
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }
                fileOk.Invoke(fileName);
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileOk"></param>
        public static void SaveFile(Action<string> fileOk)
        {
            SaveFile(FileTypeFilter.ALL, "", fileOk);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fileOk"></param>
        public static void SaveFile(FileTypeFilter filter, Action<string> fileOk)
        {
            SaveFile(filter, "", fileOk);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="defaultName"></param>
        /// <param name="fileOk"></param>
        public static void SaveFile(FileTypeFilter filter, string defaultName, Action<string> fileOk)
        {
            Debug.Assert(fileOk != null);
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = GetFilterStr(filter),
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = defaultName,
                AddExtension = true,
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string fileName = sfd.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return;
                }

                fileOk.Invoke(fileName);
            }
        }

        /// <summary>
        /// 保存文件到目录
        /// </summary>
        /// <param name="folderSelected"></param>
        public static void SaveToFolder(Action<string> folderSelected)
        {
            SaveToFolder("请选择目录", folderSelected);
        }

        /// <summary>
        /// 保存文件到目录
        /// </summary>
        /// <param name="title"></param>
        /// <param name="folderSelected"></param>
        public static void SaveToFolder(string title, Action<string> folderSelected)
        {
            Debug.Assert(folderSelected != null);
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.Desktop,
                ShowNewFolderButton = true,
                Description = title,
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                if (string.IsNullOrWhiteSpace(path))
                {
                    return;
                }

                folderSelected.Invoke(path);
            }
        }

        /// <summary>
        /// 判断DataRow是否为空行（所有列均为空或空串）
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool IsNullRow(DataRow row)
        {
            if (row == null)
            {
                return true;
            }

            for (int i = 0; i < row.ItemArray.Length; i++)
            {
                if (row[i] != null && !string.IsNullOrWhiteSpace(row[i].ToString()))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// 获取指定对象的带有指定DataTableColumn的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyInfoHolder<DataTableColumnAttribute>> GetDataColumnAttrPropertyInfos<T>()
        {
            //筛选带有FromDataRow的字段
            return GetAttributePropertyInfos<T, DataTableColumnAttribute>().OrderBy(it => it.AttributeData.ColumnIndex).ToList();
        }

        /// <summary>
        /// 获取指定对象的带有指定DgvCol的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyInfoHolder<DgvColAttribute>> GetDgvColAttrPropertyInfos<T>()
        {
            Type t = typeof(T);
            //检查类是否有DataGridBindable标记
            var attributes = t.GetCustomAttributes<DataGridBindAbleAttribute>(true);
            Debug.Assert(attributes.ToList().Count > 0, "用于DataGridView显示的类必须标记[DataGridBindAble]");
            //筛选带有DgvCol的字段
            return GetAttributePropertyInfos<T, DgvColAttribute>().OrderBy(it => it.AttributeData.Index).ToList();
        }

        /// <summary>
        /// 获取指定对象的带有指定Attribute的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <returns></returns>
        public static List<PropertyInfoHolder<TAttribute>> GetAttributePropertyInfos<T, TAttribute>()
            where TAttribute : Attribute
        {
            List<PropertyInfoHolder<TAttribute>> ret = new List<PropertyInfoHolder<TAttribute>>();
            foreach (var propertyInfo in GetPropertyInfos<T>())
            {
                var attributes = propertyInfo.GetCustomAttributes<TAttribute>();
                var colAttrs = attributes as TAttribute[] ?? attributes.ToArray();
                if (colAttrs.Count() != 1)
                {
                    continue;
                }

                ret.Add(new PropertyInfoHolder<TAttribute> { Prop = propertyInfo, AttributeData = colAttrs[0] });
            }

            return ret;
        }

        /// <summary>
        /// 获取指定对象的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        public static List<PropertyInfo> GetPropertyInfos<T>()
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

            //先获取父类属性
            Type t = typeof(T);
            if (t.BaseType != null)
            {
                var parentProperties = t.BaseType.GetProperties();
                if (parentProperties.Length > 0)
                {
                    propertyInfos.AddRange(parentProperties);
                }
            }

            //再获取子类属性
            var selfproperties = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            if (selfproperties.Length > 0)
            {
                foreach (var propertyInfo in selfproperties)
                {
                    if (propertyInfo.DeclaringType == t)
                    {
                        propertyInfos.Add(propertyInfo);
                    }
                }
            }

            return propertyInfos;
        }

        /// <summary>
        /// 获取广播地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 0xFF));
            }

            return new IPAddress(broadcastAddress);
        }

        /// <summary>
        /// 获取本机所有的ipv4地址
        /// </summary>
        /// <returns></returns>
        public static List<UnicastIPAddressInformation> GetLocalV4InterfaceInfoList()
        {
            var ret = new List<UnicastIPAddressInformation>();
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (var it in ipProps.UnicastAddresses)
                {
                    if (it.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ret.Add(it);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 判断程序是否是以管理员身份运行
        /// </summary>
        /// <returns></returns>
        public static bool IsRunAsWindowsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}