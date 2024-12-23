using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        /*
        * R e a d m e
        * -----------
        * Base graphical inventory display for LCD by Space Engineers Script.
        * 太空工程师，基地图形化脚本。
        * 
        * @version 1.0.0
        * @see <https://github.com/se-scripts/mybase>
        * @author [chivehao](https://github.com/chivehao)
        */
        /// <summary>
        /// 编程块自定义数据读取对象
        /// </summary>
        MyIni _meIni = new MyIni();
        /// <summary>
        /// 原矿箱子自定义名称标记用的标签
        /// </summary>
        string[] orgCargoNameTags = { "矿物", "原矿", "Ore", "ore" };
        /// <summary>
        /// 原矿箱子
        /// </summary>
        List<IMyCargoContainer> oreCargos = new List<IMyCargoContainer>();
        /// <summary>
        /// 矿锭箱子自定义名称标记用的标签
        /// </summary>
        string[] ingotCargoNameTags = { "矿锭", "Ingot", "ingot" };
        /// <summary>
        /// 矿锭箱子
        /// </summary>
        List<IMyCargoContainer> ingotCargos = new List<IMyCargoContainer>();
        /// <summary>
        /// 组件箱子自定义名称标记用的标签
        /// </summary>
        string[] componentCargoNameTags = { "组件", "Component", "component" };
        /// <summary>
        /// 组件箱子
        /// </summary>
        List<IMyCargoContainer> componentCargos = new List<IMyCargoContainer>();
        /// <summary>
        /// 弹药箱子自定义名称标记用的标签
        /// </summary>
        string[] ammoCargoNameTags = { "弹药", "Ammo", "ammo" };
        /// <summary>
        /// 弹药箱子
        /// </summary>
        List<IMyCargoContainer> ammoCargos = new List<IMyCargoContainer>();
        /// <summary>
        /// 不受脚本管理的箱子自定义名称标记用的标签
        /// </summary>
        string[] excludeCargoNameTags = { "排除", "Exclude", "exclude" };
        /// <summary>
        /// 不受脚本管理的箱子
        /// </summary>
        List<IMyCargoContainer> excludeCargos = new List<IMyCargoContainer>();
        /// <summary>
        /// 装配机
        /// </summary>
        List<IMyAssembler> assemblers = new List<IMyAssembler>();
        /// <summary>
        /// 精炼厂
        /// </summary>
        List<IMyRefinery> refineries = new List<IMyRefinery>();
        /// <summary>
        /// 发电设备
        /// </summary>
        List<IMyPowerProducer> powerProducers = new List<IMyPowerProducer>();
        /// <summary>
        /// 原矿箱子物品面板
        /// </summary>
        List<IMyTextPanel> orgPanels = new List<IMyTextPanel>();
        /// <summary>
        /// 矿锭箱子物品面板
        /// </summary>
        List<IMyTextPanel> ingotPanels = new List<IMyTextPanel>();
        /// <summary>
        /// 组件箱子物品面板
        /// </summary>
        List<IMyTextPanel> componentPanels = new List<IMyTextPanel>();
        /// <summary>
        /// 弹药箱子物品面板
        /// </summary>
        List<IMyTextPanel> ammoMagazinePanels = new List<IMyTextPanel>();
        /// <summary>
        /// 不受脚本管理的箱子物品展示面板
        /// </summary>
        List<IMyTextPanel> excludePanels = new List<IMyTextPanel>();





    }
}



