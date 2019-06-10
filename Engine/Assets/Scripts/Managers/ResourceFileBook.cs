//#define SIMULATE_PUBLISH  //模拟正式发布，处理资源路径和加载，在全局定义.
//#define USING_MD5_NAME  //使用md5名字，文件默认用md5名，在全局定义.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XGame
{
    public enum EExistLocation : uint
    {
        None = 0,  // 不存在
        Local = 1, // 存在本地目录
        Update,    // 存在更新目录
        LocalAndUpdate, // 同时存在本地和更新目录
    }

    // 功能：
    //1 遍历磁盘记资源文件，检查文件是否存在时无需调用IO接口 
    //2 存放文件依赖关系 
    //3 映射到md5化文件名
    public class ResourceFileBook
    {
        private static ResourceFileBook s_instance;

        private string m_productPath = "";
        private string m_localPath = ""; // 本地目录
        private string m_updatePath = ""; // 更新目录
        private string m_localUIPath = ""; // 本地ui目录
        private string m_localCachePath = ""; // 本地缓存目录


        public static ResourceFileBook instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ResourceFileBook();
                }
                return s_instance;
            }
        }

        private ResourceFileBook()
        {

        }

    }
}