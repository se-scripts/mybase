﻿using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;
using System.Linq;
using VRage;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        /*
        * R e a d m e
        * -----------
        * Base Graphical inventory display for LCD by Space Engineers Script.
        * 太空工程师，基地图形化显示库存脚本。
        * 
        * @version 1.0.0
        * @see <https://github.com/se-scripts/mybase>
        * @author [Hi.James](https://space.bilibili.com/368005035)
        * @author [chivehao](https://github.com/chivehao)
        */

        MyIni _ini = new MyIni();

        List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
        IMyTextPanel statisticsPanel = null;
        IMyTextPanel testPanel = null;
        List<IMyTextPanel> statisticsPanels = new List<IMyTextPanel>();
        List<IMyTextPanel> panels = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Items_All = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Items_Ore = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Items_Ingot = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Items_Component = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Items_AmmoMagazine = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Refineries = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Assemblers = new List<IMyTextPanel>();
        List<IMyTextPanel> panels_Overall = new List<IMyTextPanel>();
        List<IMyGasTank> oxygenTanks = new List<IMyGasTank>();
        List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
        List<IMyAssembler> assemblers = new List<IMyAssembler>();
        List<IMyRefinery> refineries = new List<IMyRefinery>();
        List<IMyPowerProducer> powerProducers = new List<IMyPowerProducer>();
        List<IMyReactor> reactors = new List<IMyReactor>();
        List<IMyGasGenerator> gasGenerators = new List<IMyGasGenerator>();
        List<string> spritesList = new List<string>();



        Dictionary<string, string> translator = new Dictionary<string, string>();
        Dictionary<string, double> productionList = new Dictionary<string, double>();

        const int itemAmountInEachScreen = 35, facilityAmountInEachScreen = 20;
        const float itemBox_ColumnInterval_Float = 73, itemBox_RowInterval_Float = 102, amountBox_Height_Float = 24, facilityBox_RowInterval_Float = 25.5f;
        const string translateList_Section = "Translate_List", length_Key = "Length";
        const string statsSection = "Stats", statsLengthKey = "Length", statsTimeIntervalKey = "StatsTimeInterval", defaultStatsTimeInterval = "5";
        int counter_ProgramRefresh = 0, counter_ShowItems = 0, counter_ShowFacilities = 0, counter_InventoryManagement = 0, counter_AssemblerManagement = 0, counter_RefineryManagement = 0, counter_Panel = 0;
        double counter_Logo = 0;
        const string basicConfigSelection = "BasicConfig"
            , isCargoSameConstructAsKey = "IsCargoSameConstructAs", defaultIsCargoSameConstructAsValue = "true"
            , isAssemblerSameConstructAsKey = "IsAssemblerSameConstructAs", defaultIsAssemblerSameConstructAsValue = "true"
            , isRefinerySameConstructAsKey = "IsRefinerySameConstructAs", defaultIsRefinerySameConstructAsValue = "true"
            , isPowerProducerSameConstructAsKey = "IsPowerProducerSameConstructAs", defaultIsPowerProducerSameConstructAsValue = "true"
            , isReactorSameConstructAsKey = "IsReactorSameConstructAs", defaultIsReactorSameConstructAsValue = "true";
        const string overallConfigSeletion = "OverallConfig", displayAssemblerCustomName = "DisplayAssemblerName";
        const string modBlueprintSubtypeIdResultMapSelection = "ModBlueprintSubtypeIdResultMap", enableKey = "enable", modBlueprintSubtypeIdResultMapLengthKey = "Length";
        Dictionary<string, string> modBlueprintSubtypeIdResultMap = new Dictionary<string, string>();


        Color background_Color = new Color(0, 35, 45);
        Color border_Color = new Color(0, 130, 255);

        public struct ItemStats
        {
            public string Name; // type id str
            public DateTime StartTime; 
            public long LastCount;
            public long Count;
            public double Difference; // 差额
        }
        List<ItemStats> itemStatsList = new List<ItemStats>();
        HashSet<string> statsItemTyps = new HashSet<string>();

        public struct ItemList
        {
            public string Name;
            public double Amount;
        }
        ItemList[] itemList_All;
        ItemList[] itemList_Ore;
        ItemList[] itemList_Ingot;
        ItemList[] itemList_Component;
        ItemList[] itemList_AmmoMagazine;


        public struct Facility_Struct
        {
            public bool IsEnabled_Bool;
            public string Name;
            public bool IsProducing_Bool;
            public bool IsCooperativeMode_Bool;
            public bool IsRepeatMode_Bool;
            public string Picture;
            public double ItemAmount;
            public string InputInventory;
            public string OutputInventory;
            public string Productivity;
        }
        Facility_Struct[] refineryList;
        Facility_Struct[] assemblerList;




        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update10;

            SetDefultConfiguration();

            BuildTranslateDic();

            BuildStatsItemTyps();

            GetBlocksFromGridTerminalSystem();

            //  incase no screen
            if (panels.Count < 1)
            {
                if (Me.SurfaceCount > 0)
                {
                    Me.GetSurface(0).GetSprites(spritesList);
                }
            }
            else
            {
                panels[0].GetSprites(spritesList);
            }




        }

        public void Save(){}

        


        public void DebugLCD(string text)
        {
            List<IMyTextPanel> debugPanel = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType(debugPanel, b => b.IsSameConstructAs(Me) && b.CustomName == "DEBUGLCD");

            if (debugPanel.Count == 0) return;

            string temp = "";
            foreach (var panel in debugPanel)
            {
                temp = "";
                temp = panel.GetText();
            }

            foreach (var panel in debugPanel)
            {
                if (panel.ContentType != ContentType.TEXT_AND_IMAGE) panel.ContentType = ContentType.TEXT_AND_IMAGE;
                panel.FontSize = 0.55f;
                panel.Font = "LoadingScreen";
                panel.WriteText(DateTime.Now.ToString(), false);
                panel.WriteText("\n", true);
                panel.WriteText(text, true);
                panel.WriteText("\n", true);
                panel.WriteText(temp, true);
            }
        }

        public void WriteConfiguration_to_CustomData(string section, string key, string value)
        {
            _ini.Set(section, key, value);
            Me.CustomData = _ini.ToString();
        }

        public void GetConfiguration_from_CustomData(string section, string key, out string value)
        {
            GetConfiguration_from_CustomData(Me.CustomData, section, key, out value);
        }

        public void GetConfiguration_from_CustomData(string customData, string section, string key, out string value)
        {

            // This time we _must_ check for failure since the user may have written invalid ini.
            MyIniParseResult result;
            if (!_ini.TryParse(customData, out result))
                throw new Exception(result.ToString());

            string DefaultValue = "";

            // Read the integer value. If it does not exist, return the default for this value.
            value = _ini.Get(section, key).ToString(DefaultValue);
        }

        public void SetDefultConfiguration()
        {
            // This time we _must_ check for failure since the user may have written invalid ini.
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());

            //  Initialize CustomData
            string dataTemp;
            dataTemp = Me.CustomData;
            if (dataTemp == "" || dataTemp == null)
            {

                _ini.Set(basicConfigSelection, isCargoSameConstructAsKey, defaultIsCargoSameConstructAsValue);
                _ini.Set(basicConfigSelection, isAssemblerSameConstructAsKey, defaultIsAssemblerSameConstructAsValue);
                _ini.Set(basicConfigSelection, isRefinerySameConstructAsKey, defaultIsRefinerySameConstructAsValue);
                _ini.Set(basicConfigSelection, isPowerProducerSameConstructAsKey, defaultIsPowerProducerSameConstructAsValue);
                _ini.Set(basicConfigSelection, isReactorSameConstructAsKey, defaultIsReactorSameConstructAsValue);

                _ini.Set(overallConfigSeletion, displayAssemblerCustomName, "");


                _ini.Set(translateList_Section, length_Key, "1");
                _ini.Set(translateList_Section, "1", "AH_BoreSight:More");

                Me.CustomData = _ini.ToString();
            }// End if

            if (!_ini.ContainsSection(statsSection)) {
                _ini.Set(statsSection, statsTimeIntervalKey, defaultStatsTimeInterval);
                _ini.Set(statsSection, statsLengthKey, 0);
                Me.CustomData = _ini.ToString();
            }
            string value;
            GetConfiguration_from_CustomData(statsSection, statsTimeIntervalKey, out value);
            statsTimeInterval = Convert.ToInt32(value);

            if (!_ini.ContainsSection(modBlueprintSubtypeIdResultMapSelection)) {
                _ini.Set(modBlueprintSubtypeIdResultMapSelection, enableKey, "false");
                _ini.Set(modBlueprintSubtypeIdResultMapSelection, modBlueprintSubtypeIdResultMapLengthKey, 0);
                Me.CustomData = _ini.ToString();
            }
            if (_ini.Get(modBlueprintSubtypeIdResultMapSelection, enableKey).ToBoolean())
            {
                var length = _ini.Get(modBlueprintSubtypeIdResultMapSelection, modBlueprintSubtypeIdResultMapLengthKey).ToInt64();
                for (var i = 1; i <= length; i++)
                {
                    var str = _ini.Get(modBlueprintSubtypeIdResultMapSelection, i.ToString()).ToString();
                    var strs = str.Split(':');
                    modBlueprintSubtypeIdResultMap.Add(strs[0], strs[1]);
                }
            }

        }


        /*###############     Overall     ###############*/
        public void OverallDisplay()
        {
            foreach (var panel in panels_Overall)
            {
                if (panel.CustomData != "0") panel.CustomData = "0";
                else panel.CustomData = "0.001";

                if (panel.ContentType != ContentType.SCRIPT) panel.ContentType = ContentType.SCRIPT;
                MySpriteDrawFrame frame = panel.DrawFrame();

                DrawContentBox(panel, frame);

                frame.Dispose();
            }
        }

        public void DrawContentBox(IMyTextPanel panel, MySpriteDrawFrame frame)
        {
            float x_Left = itemBox_ColumnInterval_Float / 2 + 1.5f, x_Right = itemBox_ColumnInterval_Float + 2 + (512 - itemBox_ColumnInterval_Float - 4) / 2, x_Title = 70, y_Title = itemBox_ColumnInterval_Float + 2 + Convert.ToSingle(panel.CustomData);
            float progressBar_YCorrect = 0f, progressBarWidth = 512 - itemBox_ColumnInterval_Float - 6, progressBarHeight = itemBox_ColumnInterval_Float - 3;

            //  Title
            DrawBox(frame, x_Left, x_Left + Convert.ToSingle(panel.CustomData), itemBox_ColumnInterval_Float, itemBox_ColumnInterval_Float, background_Color);
            DrawBox(frame, 512 - x_Left, x_Left + Convert.ToSingle(panel.CustomData), itemBox_ColumnInterval_Float, itemBox_ColumnInterval_Float, background_Color);
            DrawBox(frame, 512 / 2, x_Left + Convert.ToSingle(panel.CustomData), 512 - itemBox_ColumnInterval_Float * 2 - 4, itemBox_ColumnInterval_Float, background_Color);
            PanelWriteText(frame, panels_Overall[0].GetOwnerFactionTag(), 512 / 2, 2 + Convert.ToSingle(panel.CustomData), 2.3f, TextAlignment.CENTER);
            DrawLogo(frame, x_Left, x_Left + Convert.ToSingle(panel.CustomData), itemBox_ColumnInterval_Float);
            DrawLogo(frame, 512 - x_Left, x_Left + Convert.ToSingle(panel.CustomData), itemBox_ColumnInterval_Float);


            for (int i = 1; i <= 6; i++)
            {
                float y = i * itemBox_ColumnInterval_Float + itemBox_ColumnInterval_Float / 2 + 1.5f + Convert.ToSingle(panel.CustomData);

                DrawBox(frame, x_Left, y, itemBox_ColumnInterval_Float, itemBox_ColumnInterval_Float, background_Color);
                DrawBox(frame, x_Right, y, (512 - itemBox_ColumnInterval_Float - 4), itemBox_ColumnInterval_Float, background_Color);
            }

            //  All Cargo
            float y1 = itemBox_ColumnInterval_Float + itemBox_ColumnInterval_Float / 2 + 1.5f + Convert.ToSingle(panel.CustomData);
            MySprite sprite = MySprite.CreateSprite("Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", new Vector2(x_Left, y1), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            string percentage_String, finalValue_String;
            CalculateAll(out percentage_String, out finalValue_String);
            ProgressBar(frame, x_Right, y1 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, cargoContainers.Count.ToString(), x_Title, y_Title, 0.55f, TextAlignment.RIGHT);
            PanelWriteText(frame, percentage_String, x_Right, y_Title, 1.2f, TextAlignment.CENTER);
            PanelWriteText(frame, finalValue_String, x_Right, y_Title + itemBox_ColumnInterval_Float / 2, 1.2f, TextAlignment.CENTER);

            //  H2
            float y2 = y1 + itemBox_ColumnInterval_Float;
            sprite = MySprite.CreateSprite("IconHydrogen", new Vector2(x_Left, y2), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            CalcualateGasTank(hydrogenTanks, out percentage_String, out finalValue_String);
            PanelWriteText(frame, hydrogenTanks.Count.ToString(), x_Title, y_Title + itemBox_ColumnInterval_Float, 0.55f, TextAlignment.RIGHT);
            ProgressBar(frame, x_Right, y2 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, percentage_String, x_Right, y_Title + itemBox_ColumnInterval_Float, 1.2f, TextAlignment.CENTER);
            PanelWriteText(frame, finalValue_String, x_Right, y_Title + itemBox_ColumnInterval_Float + itemBox_ColumnInterval_Float / 2, 1.2f, TextAlignment.CENTER);

            //  O2
            float y3 = y2 + itemBox_ColumnInterval_Float;
            sprite = MySprite.CreateSprite("IconOxygen", new Vector2(x_Left, y3), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            CalcualateGasTank(oxygenTanks, out percentage_String, out finalValue_String);
            PanelWriteText(frame, oxygenTanks.Count.ToString(), x_Title, y_Title + itemBox_ColumnInterval_Float * 2, 0.55f, TextAlignment.RIGHT);
            ProgressBar(frame, x_Right, y3 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, percentage_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 2, 1.2f, TextAlignment.CENTER);
            PanelWriteText(frame, finalValue_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 2 + itemBox_ColumnInterval_Float / 2, 1.2f, TextAlignment.CENTER);

            //  Power
            float y4 = y3 + itemBox_ColumnInterval_Float;
            sprite = MySprite.CreateSprite("IconEnergy", new Vector2(x_Left, y4), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            CalculatePowerProducer(out percentage_String, out finalValue_String);
            PanelWriteText(frame, powerProducers.Count.ToString(), x_Title, y_Title + itemBox_ColumnInterval_Float * 3, 0.55f, TextAlignment.RIGHT);
            ProgressBar(frame, x_Right, y4 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, percentage_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 3, 1.2f, TextAlignment.CENTER);
            PanelWriteText(frame, finalValue_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 3 + itemBox_ColumnInterval_Float / 2, 1.2f, TextAlignment.CENTER);

            // 特殊生产设备的生产进度
            float y5 = y4 + itemBox_ColumnInterval_Float;
            sprite = MySprite.CreateSprite("Textures\\FactionLogo\\Builders\\BuilderIcon_16.dds", new Vector2(x_Left, y5), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            string itemName;
            CalculateSpProducerProgress(out itemName, out percentage_String, out finalValue_String);
            PanelWriteText(frame, "", x_Title, y_Title + itemBox_ColumnInterval_Float * 4, 0.55f, TextAlignment.RIGHT);
            ProgressBar(frame, x_Right, y5 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, itemName, x_Right, y_Title + itemBox_ColumnInterval_Float * 4, 1.2f, TextAlignment.CENTER);
            PanelWriteText(frame, finalValue_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 4 + itemBox_ColumnInterval_Float / 2, 1.2f, TextAlignment.CENTER);


        }

        public void ProgressBar(MySpriteDrawFrame frame, float x, float y, float width, float height, string ratio)
        {
            string[] ratiogroup = ratio.Split('%');
            float ratio_Float = Convert.ToSingle(ratiogroup[0]);
            float currentWidth = width * ratio_Float / 100;
            float currentX = x - width / 2 + currentWidth / 2;

            Color co = new Color(0, 0, 256);

            if (ratio_Float == 0) return;

            DrawBox(frame, currentX, y, currentWidth, height, co, co);
        }

        public void DrawLogo(MySpriteDrawFrame frame, float x, float y, float width)
        {
                MySprite sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Screen_LoadingBar",
                    Position = new Vector2(x, y),
                    Size = new Vector2(width - 6, width - 6),
                    RotationOrScale = Convert.ToSingle(counter_Logo / 360 * 2 * Math.PI),
                    Alignment = TextAlignment.CENTER,
                };
                frame.Add(sprite);

                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Screen_LoadingBar",
                    Position = new Vector2(x, y),
                    Size = new Vector2(width / 2, width / 2),
                    RotationOrScale = Convert.ToSingle(2 * Math.PI - counter_Logo / 360 * 2 * Math.PI),
                    Alignment = TextAlignment.CENTER,
                };
                frame.Add(sprite);

                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "Screen_LoadingBar",
                    Position = new Vector2(x, y),
                    Size = new Vector2(width / 4, width / 4),
                    RotationOrScale = Convert.ToSingle(Math.PI + counter_Logo / 360 * 2 * Math.PI),
                    Alignment = TextAlignment.CENTER,
                };
                frame.Add(sprite);

            }
            
        public void CalculateAll(out string percentage_String, out string finalValue_String)
        {
            double currentVolume_Double = 0, totalVolume_Double = 0;

            foreach (var cargoContainer in cargoContainers)
            {
                currentVolume_Double += ((double)cargoContainer.GetInventory().CurrentVolume);
                totalVolume_Double += ((double)cargoContainer.GetInventory().MaxVolume);
            }

            percentage_String = Math.Round(currentVolume_Double / totalVolume_Double * 100, 1).ToString() + "%";
            finalValue_String = AmountUnitConversion(currentVolume_Double * 1000) + " L / " + AmountUnitConversion(totalVolume_Double * 1000) + " L";
        }

        public void CalcualateGasTank(List<IMyGasTank> tanks, out string percentage_String, out string finalValue_String)
        {
            double currentVolume_Double = 0, totalVolume_Double = 0;

            foreach (var tank in tanks)
            {
                currentVolume_Double += tank.Capacity * tank.FilledRatio;
                totalVolume_Double += tank.Capacity;
            }

            percentage_String = Math.Round(currentVolume_Double / totalVolume_Double * 100, 1).ToString() + "%";
            finalValue_String = AmountUnitConversion(currentVolume_Double) + " L / " + AmountUnitConversion(totalVolume_Double) + " L";
        }

        public void CalculatePowerProducer(out string percentage_String, out string finalValue_String)
        {
            double currentOutput = 0, totalOutput = 0;
            foreach (var powerProducer in powerProducers)
            {
                currentOutput += powerProducer.CurrentOutput;
                totalOutput += powerProducer.MaxOutput;
            }

            percentage_String = Math.Round(currentOutput / totalOutput * 100, 1).ToString() + "%";
            finalValue_String = AmountUnitConversion(currentOutput * 1000000) + " W / " + AmountUnitConversion(totalOutput * 1000000) + " W";
        }


        public void CalculateSpProducerProgress(out string producer_Name, out string percentage_String, out string finalValue_String)
        {
            producer_Name = "";
            percentage_String = 0 + "%";
            finalValue_String = 0 + " % / " + 100 + " W";
            GetConfiguration_from_CustomData(overallConfigSeletion, displayAssemblerCustomName, out producer_Name);
            if (producer_Name != "")
            {
                string name = producer_Name;
                List<IMyAssembler> asses = assemblers.Where((ass) => ass.CustomName.Contains(name)).ToList();
                if (asses.Count > 0)
                {
                    IMyAssembler ass = asses[0];
                    producer_Name = ass.CustomName;
                    percentage_String = Math.Round(ass.CurrentProgress * 100, 4) + "%";
                    finalValue_String = Math.Round(ass.CurrentProgress * 100, 4) + " / 100%";
                }
            }
        }




        //###############     Overall     ###############



        /*###############     ShowItems     ###############*/

        public void ShowItems()
        {
            //GetAllItems();

            if (counter_ShowItems >= 7) counter_ShowItems = 1;

            switch (counter_ShowItems.ToString())
            {
                case "1":
                    GetAllItems();
                    break;
                case "2":
                    ItemDivideInGroups(itemList_All, panels_Items_All);
                    break;
                case "3":
                    ItemDivideInGroups(itemList_Ore, panels_Items_Ore);
                    break;
                case "4":
                    ItemDivideInGroups(itemList_Ingot, panels_Items_Ingot);
                    break;
                case "5":
                    ItemDivideInGroups(itemList_Component, panels_Items_Component);
                    break;
                case "6":
                    ItemDivideInGroups(itemList_AmmoMagazine, panels_Items_AmmoMagazine);
                    break;
            }

            counter_ShowItems++;
        }

        public void BuildTranslateDic()
        {
            string value;
            GetConfiguration_from_CustomData(translateList_Section, length_Key, out value);
            int length = Convert.ToInt16(value);

            for (int i = 1; i <= length; i++)
            {
                GetConfiguration_from_CustomData(translateList_Section, i.ToString(), out value);
                string[] result = value.Split(':');

                translator.Add(result[0], result[1]);
            }
        }

        public void BuildStatsItemTyps() {
            string value;
            GetConfiguration_from_CustomData(statsSection, statsLengthKey, out value);
            int length = Convert.ToInt16(value);

            for (int i = 1; i <= length; i++)
            {
                GetConfiguration_from_CustomData(statsSection, i.ToString(), out value);
                statsItemTyps.Add(value);
            }
        }

        public void GetBlocksFromGridTerminalSystem() {
            string isCargoSameConstructAsStr = defaultIsCargoSameConstructAsValue;
            GetConfiguration_from_CustomData(basicConfigSelection, isCargoSameConstructAsKey, out isCargoSameConstructAsStr);
            bool isCargoSameConstructAs = (isCargoSameConstructAsStr == "true");

            cargoContainers.Clear();
            statisticsPanels.Clear();
            panels.Clear();
            panels_Overall.Clear();
            panels_Items_All.Clear();
            panels_Items_Ore.Clear();
            panels_Items_Ingot.Clear();
            panels_Items_Component.Clear();
            panels_Items_AmmoMagazine.Clear();
            panels_Assemblers.Clear();
            panels_Refineries.Clear();
            oxygenTanks.Clear();
            hydrogenTanks.Clear();
            assemblers.Clear();
            refineries.Clear();
            powerProducers.Clear();
            reactors.Clear();
            gasGenerators.Clear();

            GridTerminalSystem.GetBlocksOfType(cargoContainers, b => (isCargoSameConstructAs ? b.IsSameConstructAs(Me) : true));

            GridTerminalSystem.GetBlocksOfType(panels, b => b.IsSameConstructAs(Me));
            var sPanRes = GridTerminalSystem.GetBlockWithName("LCD_Statistics");
            if (sPanRes != null) statisticsPanel = (IMyTextPanel)sPanRes;
            var tPanRes = GridTerminalSystem.GetBlockWithName("TEST");
            if (tPanRes != null) testPanel = (IMyTextPanel) tPanRes;
            GridTerminalSystem.GetBlocksOfType(statisticsPanels, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Statistics_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Overall, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Overall_Display"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_All, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Ore, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Ore_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Ingot, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Ingot_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Component, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Component_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_AmmoMagazine, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_AmmoMagazine_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Assemblers, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Assembler_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Refineries, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Refinery_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(oxygenTanks, b => b.IsSameConstructAs(Me) && !b.DefinitionDisplayNameText.ToString().Contains("Hydrogen") && !b.DefinitionDisplayNameText.ToString().Contains("氢气"));
            GridTerminalSystem.GetBlocksOfType(hydrogenTanks, b => b.IsSameConstructAs(Me) && !b.DefinitionDisplayNameText.ToString().Contains("Oxygen") && !b.DefinitionDisplayNameText.ToString().Contains("氧气"));

            string isAssemblerSameConstructAsStr = defaultIsAssemblerSameConstructAsValue;
            GetConfiguration_from_CustomData(basicConfigSelection, isAssemblerSameConstructAsKey, out isAssemblerSameConstructAsStr);
            bool isAssemblerSameConstructAs = (isAssemblerSameConstructAsStr == "true");
            GridTerminalSystem.GetBlocksOfType(assemblers, b => (isAssemblerSameConstructAs ? b.IsSameConstructAs(Me) : true));
            assemblers = assemblers.OrderBy(e => e.CustomName).ToList();

            string isRefinerySameConstructAsStr = defaultIsRefinerySameConstructAsValue;
            GetConfiguration_from_CustomData(basicConfigSelection, isRefinerySameConstructAsKey, out isRefinerySameConstructAsStr);
            bool isRefinerySameConstructAs = (isRefinerySameConstructAsStr == "true");
            GridTerminalSystem.GetBlocksOfType(refineries, b => !b.BlockDefinition.ToString().Contains("Shield") && (isRefinerySameConstructAs ? b.IsSameConstructAs(Me) : true));
            refineries = refineries.OrderBy(e => e.CustomName).ToList();

            string isPowerProducerSameConstructAsStr = defaultIsPowerProducerSameConstructAsValue;
            GetConfiguration_from_CustomData(basicConfigSelection, isPowerProducerSameConstructAsKey, out isPowerProducerSameConstructAsStr);
            bool isPowerProducerSameConstructAs = (isPowerProducerSameConstructAsStr == "true");
            GridTerminalSystem.GetBlocksOfType(powerProducers, b => (isPowerProducerSameConstructAs ? b.IsSameConstructAs(Me) : true));

            string isReactorSameConstructAsStr = defaultIsReactorSameConstructAsValue;
            GetConfiguration_from_CustomData(basicConfigSelection, isReactorSameConstructAsKey, out isReactorSameConstructAsStr);
            bool isReactorSameConstructAs = (isReactorSameConstructAsStr == "true");
            GridTerminalSystem.GetBlocksOfType(reactors, b => (isReactorSameConstructAs ? b.IsSameConstructAs(Me) : true));

            GridTerminalSystem.GetBlocksOfType(gasGenerators, b => b.IsSameConstructAs(Me));

        }

        public void GetAllItems()
        {
            Dictionary<string, double> allItems = new Dictionary<string, double>();

            foreach (var cargoContainer in cargoContainers)
            {
                var items = new List<MyInventoryItem>();
                cargoContainer.GetInventory().GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var cargoContainer in oxygenTanks)
            {
                var items = new List<MyInventoryItem>();
                cargoContainer.GetInventory().GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var cargoContainer in hydrogenTanks)
            {
                var items = new List<MyInventoryItem>();
                cargoContainer.GetInventory().GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var reactor in reactors)
            {
                var items = new List<MyInventoryItem>();
                reactor.GetInventory().GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var cargoContainer in assemblers)
            {
                var items = new List<MyInventoryItem>();
                cargoContainer.OutputInventory.GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var cargoContainer in refineries)
            {
                var items = new List<MyInventoryItem>();
                cargoContainer.InputInventory.GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }
            }

            foreach (var gasGenerator in gasGenerators)
            {
                var items = new List<MyInventoryItem>();
                gasGenerator.GetInventory().GetItems(items);

                foreach (var item in items)
                {
                    if (allItems.ContainsKey(item.Type.ToString())) allItems[item.Type.ToString()] += (double)item.Amount.RawValue;
                    else allItems.Add(item.Type.ToString(), (double)item.Amount.RawValue);
                }

            }

            itemList_All = new ItemList[allItems.Count];

            int k = 0;
            foreach (var key in allItems.Keys)
            {
                itemList_All[k].Name = key;
                itemList_All[k].Amount = allItems[key];
                k++;
            }

            itemList_Ore = new ItemList[LengthOfEachCategory("MyObjectBuilder_Ore")];
            itemList_Ingot = new ItemList[LengthOfEachCategory("MyObjectBuilder_Ingot")];
            itemList_AmmoMagazine = new ItemList[LengthOfEachCategory("MyObjectBuilder_AmmoMagazine")];

            transferItemsList(itemList_Ore, "MyObjectBuilder_Ore");
            transferItemsList(itemList_Ingot, "MyObjectBuilder_Ingot");
            transferItemsList(itemList_AmmoMagazine, "MyObjectBuilder_AmmoMagazine");

            itemList_Component = new ItemList[itemList_All.Length - itemList_Ore.Length - itemList_Ingot.Length - itemList_AmmoMagazine.Length];

            k = 0;
            foreach (var item in itemList_All)
            {
                if (item.Name.IndexOf("MyObjectBuilder_Ore") == -1 && item.Name.IndexOf("MyObjectBuilder_Ingot") == -1 && item.Name.IndexOf("MyObjectBuilder_AmmoMagazine") == -1)
                {
                    itemList_Component[k].Name = item.Name;
                    itemList_Component[k].Amount = item.Amount;
                    k++;
                }
            }

        }

        public int LengthOfEachCategory(string tag)
        {
            Dictionary<string, double> keyValuePairs = new Dictionary<string, double>();

            foreach (var item in itemList_All)
            {
                if (item.Name.IndexOf(tag) != -1)
                {
                    keyValuePairs.Add(item.Name, item.Amount);
                }
            }

            return keyValuePairs.Count;
        }

        public void transferItemsList(ItemList[] itemList, string tag)
        {
            int k = 0;
            foreach (var item in itemList_All)
            {
                if (item.Name.IndexOf(tag) != -1)
                {
                    itemList[k].Name = item.Name;
                    itemList[k].Amount = item.Amount;
                    k++;
                }
            }
        }

        public void ItemDivideInGroups(ItemList[] itemList, List<IMyTextPanel> panels_Items)
        {
            if (itemList.Length == 0 || panels_Items.Count == 0) return;

            //  get all panel numbers
            int[] findMax = new int[panels_Items.Count];
            int k = 0;
            foreach (var panel in panels_Items)
            {
                //  get current panel number
                string[] arry = panel.CustomName.Split(':');
                findMax[k] = Convert.ToInt16(arry[1]);
                k++;
            }

            if (itemList.Length > FindMax(findMax) * itemAmountInEachScreen)
            {
                foreach (var panel in panels_Items)
                {
                    if (panel.CustomData != "0") panel.CustomData = "0";
                    else panel.CustomData = "1";

                    panel.ContentType = ContentType.SCRIPT;
                    panel.BackgroundColor = Color.Black;
                    MySpriteDrawFrame frame = panel.DrawFrame();
                    string[] arry = panel.CustomName.Split(':');
                    if (Convert.ToInt16(arry[1]) < FindMax(findMax))
                    {
                        DrawFullItemScreen(panel, frame, arry[1], true, itemList);
                    }
                    else
                    {
                        DrawFullItemScreen(panel, frame, arry[1], false, itemList);
                    }
                    frame.Dispose();
                }
            }
            else
            {
                foreach (var panel in panels_Items)
                {
                    if (panel.CustomData != "0") panel.CustomData = "0";
                    else panel.CustomData = "1";

                    panel.ContentType = ContentType.SCRIPT;
                    panel.BackgroundColor = Color.Black;
                    MySpriteDrawFrame frame = panel.DrawFrame();
                    string[] arry = panel.CustomName.Split(':');
                    DrawFullItemScreen(panel, frame, arry[1], true, itemList);
                    frame.Dispose();
                }
            }
        }

        public int FindMax(int[] arry)
        {
            int p = 0;
            for (int i = 0; i < arry.Length; i++)
            {
                if (i == 0) p = arry[i];
                else if (arry[i] > p) p = arry[i];
            }

            return p;
        }

        public void DrawFullItemScreen(IMyTextPanel panel, MySpriteDrawFrame frame, string groupNumber, bool isEnoughScreen, ItemList[] itemList)
        {
            panel.WriteText("", false);

            DrawBox(frame, 512 / 2, 512 / 2 + Convert.ToSingle(panel.CustomData), 520, 520, new Color(0, 0, 0));

            for (int i = 0; i < itemAmountInEachScreen; i++)
            {
                int k = (Convert.ToInt16(groupNumber) - 1) * itemAmountInEachScreen + i;
                int x = (i + 1) % 7;
                if (x == 0) x = 7;
                int y = Convert.ToInt16(Math.Ceiling(Convert.ToDecimal(Convert.ToDouble(i + 1) / 7)));

                if (k > itemList.Length - 1)
                {
                    return;
                }
                else
                {
                    if (x == 7 && y == 5)
                    {
                        if (isEnoughScreen)
                        {
                            DrawSingleItemUnit(panel, frame, itemList[k].Name, itemList[k].Amount / 1000000, x, y);
                        }
                        else
                        {
                            double residus = itemList.Length - itemAmountInEachScreen * Convert.ToInt16(groupNumber) + 1;
                            DrawSingleItemUnit(panel, frame, "AH_BoreSight", residus, x, y);
                        }
                    }
                    else
                    {
                        DrawSingleItemUnit(panel, frame, itemList[k].Name, itemList[k].Amount / 1000000, x, y);
                    }

                    panel.WriteText(itemList[k].Name, true);
                    panel.WriteText("\n", true);

                }

            }
        }

        public void DrawSingleItemUnit(IMyTextPanel panel, MySpriteDrawFrame frame, string itemName, double amount, float x, float y)
        {

            //  Picture box
            float x1 = Convert.ToSingle((x - 1) * itemBox_ColumnInterval_Float + (itemBox_ColumnInterval_Float - 1) / 2 + 1.25f);
            float y1 = Convert.ToSingle((y - 1) * itemBox_RowInterval_Float + (itemBox_RowInterval_Float - 1) / 2 + 1.5f) + Convert.ToSingle(panel.CustomData);
            //DrawBox(frame, x1, y1, itemBox_ColumnInterval_Float, itemBox_RowInterval_Float, border_Color, background_Color);
            DrawBox(frame, x1, y1, itemBox_ColumnInterval_Float, itemBox_RowInterval_Float, background_Color);
            MySprite sprite = MySprite.CreateSprite(itemName, new Vector2(x1, y1 - 3), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);

            //  Amount box
            float y_Border_Amount = y1 + (itemBox_ColumnInterval_Float - 1) / 2 + amountBox_Height_Float / 2 - 1;
            //DrawBox(frame, x1, y_Border_Amount, itemBox_ColumnInterval_Float, amountBox_Height_Float, border_Color, background_Color);

            //  Amount text
            float x_Text_Amount = x1 + (itemBox_ColumnInterval_Float - 3) / 2 - 1;
            float y_Text_Amount = y1 + itemBox_RowInterval_Float / 2 - amountBox_Height_Float;
            PanelWriteText(frame, AmountUnitConversion(amount), x_Text_Amount, y_Text_Amount, 0.8f, TextAlignment.RIGHT);

            //  Name text
            float x_Name = x1 - (itemBox_ColumnInterval_Float - 3) / 2 + 1;
            float y_Name = y1 - (itemBox_RowInterval_Float - 3) / 2 + 1;
            PanelWriteText(frame, TranslateName(itemName), x_Name, y_Name, 0.55f, TextAlignment.LEFT);
        }

        public void PanelWriteText(MySpriteDrawFrame frame, string text, float x, float y, float fontSize, TextAlignment alignment)
        {
            MySprite sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = text,
                Position = new Vector2(x, y),
                RotationOrScale = fontSize,
                Color = Color.Coral,
                Alignment = alignment,
                FontId = "LoadingScreen"
            };
            frame.Add(sprite);
        }

        public void PanelWriteText(MySpriteDrawFrame frame, string text, float x, float y, float fontSize, TextAlignment alignment, Color co)
        {
            MySprite sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = text,
                Position = new Vector2(x, y),
                RotationOrScale = fontSize,
                Color = co,
                Alignment = alignment,
                FontId = "LoadingScreen"
            };
            frame.Add(sprite);
        }

        public void DrawBox(MySpriteDrawFrame frame, float x, float y, float width, float height, Color border_Color, Color background_Color)
        {
            //Echo($"width={width} | height={height}");


            MySprite sprite;

            sprite = MySprite.CreateSprite("SquareSimple", new Vector2(x, y), new Vector2(width - 1, height - 1));
            sprite.Color = border_Color;
            frame.Add(sprite);

            sprite = MySprite.CreateSprite("SquareSimple", new Vector2(x, y), new Vector2(width - 3, height - 3));
            sprite.Color = background_Color;
            frame.Add(sprite);
        }

        public void DrawBox(MySpriteDrawFrame frame, float x, float y, float width, float height, Color background_Color)
        {
            MySprite sprite;
            sprite = MySprite.CreateSprite("SquareSimple", new Vector2(x, y), new Vector2(width - 2, height - 2));
            sprite.Color = background_Color;
            frame.Add(sprite);
        }

        public string AmountUnitConversion(double amount)
        {
            double temp = 0;
            string result = "";

            if (amount >= 1000000000000000)
            {
                temp = Math.Round(amount / 1000000000000000, 1);
                result = temp.ToString() + "KT";
            }
            else if (amount >= 1000000000000)
            {
                temp = Math.Round(amount / 1000000000000, 1);
                result = temp.ToString() + "T";
            }
            else if (amount >= 1000000000)
            {
                temp = Math.Round(amount / 1000000000, 1);
                result = temp.ToString() + "G";
            }
            else if (amount >= 1000000)
            {
                temp = Math.Round(amount / 1000000, 1);
                result = temp.ToString() + "M";
            }
            else if (amount >= 1000)
            {
                temp = Math.Round(amount / 1000, 1);
                result = temp.ToString() + "K";
            }
            else
            {
                temp = Math.Round(amount, 1);
                result = temp.ToString();
            }

            return result;
        }

        public string ShortName(string name)
        {
            string[] temp = name.Split('/');

            if (temp.Length == 2)
            {
                return temp[1];
            }
            else
            {
                return name;
            }
        }

        public string TranslateName(string name)
        {
            if (translator.ContainsKey(name))
            {
                return translator[name];
            }
            else
            {
                return ShortName(name);
            }
        }

        //###############     ShowItems     ###############

        /*###############   RefineryAndAssembler    ###############*/

        public void ShowFacilities()
        {
            if (counter_ShowFacilities >= panels_Refineries.Count + panels_Assemblers.Count + 2) counter_ShowFacilities = 0;

            GetFacilities();

            if (counter_ShowFacilities <= panels_Refineries.Count)
            {
                if (refineries.Count > 0) FacilitiesDivideIntoGroup(refineryList, panels_Refineries);
            }
            else
            {
                if (assemblers.Count > 0) FacilitiesDivideIntoGroup(assemblerList, panels_Assemblers);
            }

            counter_ShowFacilities++;
        }

        public void GetFacilities()
        {

            refineryList = new Facility_Struct[refineries.Count];

            int k = 0;
            foreach (var refinery in refineries)
            {
                refineryList[k].Name = refinery.CustomName;
                refineryList[k].IsEnabled_Bool = refinery.Enabled;
                refineryList[k].IsProducing_Bool = refinery.IsProducing;
                refineryList[k].IsCooperativeMode_Bool = false;
                refineryList[k].IsRepeatMode_Bool = false;


                List<MyInventoryItem> items = new List<MyInventoryItem>();
                refinery.InputInventory.GetItems(items);
                if (items.Count == 0)
                {
                    refineryList[k].Picture = "Empty";
                    refineryList[k].ItemAmount = 0;
                }
                else
                {
                    refineryList[k].Picture = items[0].Type.ToString();
                    refineryList[k].ItemAmount = (double)items[0].Amount;
                }

                char[] delimiterChars = { ':', '：' };
                string[] str1 = refinery.DetailedInfo.Split('%');
                string[] str2 = str1[0].Split(delimiterChars);
                refineryList[k].Productivity = str2[str2.Length - 1];

                k++;
            }

            assemblerList = new Facility_Struct[assemblers.Count];

            k = 0;
            foreach (var assembler in assemblers)
            {
                assemblerList[k].Name = assembler.CustomName;
                assemblerList[k].IsEnabled_Bool = assembler.Enabled;
                assemblerList[k].IsProducing_Bool = assembler.IsProducing;
                assemblerList[k].IsCooperativeMode_Bool = assembler.CooperativeMode;
                assemblerList[k].IsRepeatMode_Bool = assembler.Repeating;


                List<MyProductionItem> items = new List<MyProductionItem>();
                assembler.GetQueue(items);
                if (items.Count == 0)
                {
                    assemblerList[k].Picture = "Empty";
                    assemblerList[k].ItemAmount = 0;
                }
                else
                {
                    assemblerList[k].Picture = items[0].BlueprintId.ToString();
                    assemblerList[k].ItemAmount = (double)items[0].Amount;
                }


                char[] delimiterChars = { ':', '：' };
                string[] str1 = assembler.DetailedInfo.Split('%');
                string[] str2 = str1[0].Split(delimiterChars);
                assemblerList[k].Productivity = str2[str2.Length - 1];

                k++;
            }
        }

        public void FacilitiesDivideIntoGroup(Facility_Struct[] facilityList, List<IMyTextPanel> facilityPanels)
        {
            if (facilityList.Length == 0 || facilityPanels.Count == 0) return;

            //  get all panel numbers
            int[] findMax = new int[facilityPanels.Count];
            int k = 0;
            foreach (var panel in facilityPanels)
            {
                //  get current panel number
                string[] arry = panel.CustomName.Split(':');
                findMax[k] = Convert.ToInt16(arry[1]);
                k++;
            }

            if (counter_Panel >= facilityPanels.Count) counter_Panel = 0;

            if (facilityList.Length > FindMax(findMax) * facilityAmountInEachScreen)
            {
                //  Not enough panel
                var panel = facilityPanels[counter_Panel];

                if (panel.CustomData != "0") panel.CustomData = "0";
                else panel.CustomData = "1";

                if (panel.ContentType != ContentType.SCRIPT) panel.ContentType = ContentType.SCRIPT;
                MySpriteDrawFrame frame = panel.DrawFrame();
                string[] arry = panel.CustomName.Split(':');
                if (Convert.ToInt16(arry[1]) < FindMax(findMax))
                {
                    DrawFullFacilityScreen(panel, frame, arry[1], true, facilityList);
                }
                else
                {
                    DrawFullFacilityScreen(panel, frame, arry[1], false, facilityList);
                }
                frame.Dispose();

                Echo(panel.CustomName);
            }
            else
            {
                //  Enough panel
                var panel = facilityPanels[counter_Panel];

                if (panel.CustomData != "0") panel.CustomData = "0";
                else panel.CustomData = "1";

                if (panel.ContentType != ContentType.SCRIPT) panel.ContentType = ContentType.SCRIPT;
                MySpriteDrawFrame frame = panel.DrawFrame();
                string[] arry = panel.CustomName.Split(':');
                DrawFullFacilityScreen(panel, frame, arry[1], true, facilityList);
                frame.Dispose();

                Echo(panel.CustomName);
            }

            counter_Panel++;
        }

        public void DrawFullFacilityScreen(IMyTextPanel panel, MySpriteDrawFrame frame, string groupNumber, bool isEnoughScreen, Facility_Struct[] facilityList)
        {
            panel.WriteText("", false);

            DrawFacilityScreenFrame(panel, frame);

            for (int i = 0; i < facilityAmountInEachScreen; i++)
            {
                int k = (Convert.ToInt16(groupNumber) - 1) * facilityAmountInEachScreen + i;

                if (k > facilityList.Length - 1) return;//Last facility is finished.

                if (i == facilityAmountInEachScreen - 1)
                {
                    if (isEnoughScreen)
                    {
                        DrawSingleFacilityUnit(panel, frame, (k + 1).ToString() + ". " + facilityList[k].Name + " ×" + facilityList[k].Productivity + "%", facilityList[k].IsProducing_Bool, AmountUnitConversion(facilityList[k].ItemAmount), facilityList[k].Picture, facilityList[k].IsRepeatMode_Bool, facilityList[k].IsCooperativeMode_Bool, facilityList[k].IsEnabled_Bool, i);
                    }
                    else
                    {
                        double residus = facilityList.Length - facilityAmountInEachScreen * Convert.ToInt16(groupNumber) + 1;
                        DrawSingleFacilityUnit(panel, frame, "+ " + residus.ToString() + " Facilities", false, "0", "Empty", false, false, false, i);
                    }
                }
                else
                {
                    DrawSingleFacilityUnit(panel, frame, (k + 1).ToString() + ". " + facilityList[k].Name + " ×" + facilityList[k].Productivity + "%", facilityList[k].IsProducing_Bool, AmountUnitConversion(facilityList[k].ItemAmount), facilityList[k].Picture, facilityList[k].IsRepeatMode_Bool, facilityList[k].IsCooperativeMode_Bool, facilityList[k].IsEnabled_Bool, i);
                }

                panel.WriteText($"{(k + 1).ToString() + ".\n"}{facilityList[k].Name}\n{facilityList[k].Picture}", true);
                panel.WriteText("\n\n", true);
            }
        }

        public void DrawFacilityScreenFrame(IMyTextPanel panel, MySpriteDrawFrame frame)
        {
            float lineWith_FLoat = 1f;

            //DrawBox(frame, 512 / 2, 512 / 2 + Convert.ToSingle(panel.CustomData), 514, 514, Color.Black);
            DrawBox(frame, 512 / 2, 512 / 2 + Convert.ToSingle(panel.CustomData), 514, 514, background_Color);

            for (int i = 0; i <= 20; i++)
            {
                DrawBox(frame, 512 / 2, 1 + facilityBox_RowInterval_Float * i, 512, lineWith_FLoat, Color.Black);
            }

            float x1 = 1, x2 = x1 + 92, x3 = x2 + facilityBox_RowInterval_Float, x4 = x3 + facilityBox_RowInterval_Float, x5 = 512, x31 = (x3 + x4) / 2;
            DrawBox(frame, x1, 512 / 2, lineWith_FLoat, 512, Color.Black);
            DrawBox(frame, x2, 512 / 2, lineWith_FLoat, 512, Color.Black);
            DrawBox(frame, x31, 512 / 2, facilityBox_RowInterval_Float + 2, 512, Color.Black);
            DrawBox(frame, x5, 512 / 2, lineWith_FLoat, 512, Color.Black);

        }

        public void DrawSingleFacilityUnit(IMyTextPanel panel, MySpriteDrawFrame frame, string Name, bool isProducing, string itemAmount, string picture, bool isRepeating, bool isCooperative, bool isEnabled, int index)
        {
            //  ItemAmount box
            float h = facilityBox_RowInterval_Float;
            float width = 92f;
            float x1 = Convert.ToSingle(1 + width / 2);
            float y1 = Convert.ToSingle(1 + h / 2 + h * index) + Convert.ToSingle(panel.CustomData);
            float textY = y1 - h / 2 + 2F, textSize = 0.75f;
            //DrawBox(frame, x1, y1, width, h, background_Color);
            if (isRepeating) PanelWriteText(frame, "RE", x1 - width / 2 + 2f, textY, textSize, TextAlignment.LEFT);
            PanelWriteText(frame, itemAmount, x1 + width / 2 - 2f, textY, textSize, TextAlignment.RIGHT);

            //  picture box
            float x2 = x1 + width / 2 + h / 2 + 0.5f;
            float boxWH_Float = h - 1;
            //DrawBox(frame, x2, y1, h, h, background_Color);
            MySprite sprite;
            if (picture != "Empty")
            {
                sprite = MySprite.CreateSprite(TranslateSpriteName(picture), new Vector2(x2, y1), new Vector2(boxWH_Float, boxWH_Float));
                frame.Add(sprite);
            }

            //  isproduction box
            float x3 = x2 + h - 0.5f;
            DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, background_Color);
            if (isEnabled)
            {
                if (isProducing)
                {
                    DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(0, 140, 0));
                    sprite = MySprite.CreateSprite("Textures\\FactionLogo\\Builders\\BuilderIcon_7.dds", new Vector2(x3, y1), new Vector2(boxWH_Float, boxWH_Float));
                    frame.Add(sprite);
                }
                else {
                    DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(130, 100, 0));
                    if (itemAmount != "0") {
                        sprite = MySprite.CreateSprite("Danger", new Vector2(x3, y1), new Vector2(boxWH_Float, boxWH_Float));
                        frame.Add(sprite);
                    }
                }
            }
            else
            {
                DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(178, 9, 9));
                sprite = MySprite.CreateSprite("Cross", new Vector2(x3, y1), new Vector2(boxWH_Float, boxWH_Float));
                frame.Add(sprite);
            }
            if (isCooperative) DrawImage(frame, "Circle", x3, y1, h - 7, new Color(0, 0, 255));


            //  name box
            float nameBoxWidth = Convert.ToSingle((512 - x3 - h / 2) - 2);
            float x4 = x3 + h / 2 + nameBoxWidth / 2 + 0.5f;
            //DrawBox(frame, x4, y1, nameBoxWidth, h, background_Color);
            PanelWriteText(frame, Name, x4 - nameBoxWidth / 2 + 1f, textY, textSize, TextAlignment.LEFT);
        }

        public string TranslateSpriteName(string name)
        {
            string[] blueprintIds = name.Split('/');
            var blueprintSubtypeId = blueprintIds[blueprintIds.Length - 1];

            if (modBlueprintSubtypeIdResultMap.Count > 0 && modBlueprintSubtypeIdResultMap.ContainsKey(blueprintSubtypeId)) {
                blueprintSubtypeId = modBlueprintSubtypeIdResultMap[blueprintSubtypeId];
            }

            string temp = "Textures\\FactionLogo\\Empty.dds";
            foreach (var sprite in spritesList)
            {
                if (sprite.IndexOf(blueprintSubtypeId) != -1)
                {
                    temp = sprite;
                    break;
                }
            }
            return temp;
        }

        public void DrawImage(MySpriteDrawFrame frame, string name, float x, float y, float width, Color co)
        {
            MySprite sprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = name,
                Position = new Vector2(x, y),
                Size = new Vector2(width - 6, width - 6),
                Color = co,
                Alignment = TextAlignment.CENTER,
            };
            frame.Add(sprite);
        }


        //###############   RefineryAndAssembler    ###############



        /*###############     ManageInventory     ###############*/

        public void ManageInventory()
        {
            if (counter_InventoryManagement++ >= 13) counter_InventoryManagement = 1;

            switch (counter_InventoryManagement)
            {
                case 1:
                    Assembler_to_CargoContainers();
                    break;
                case 2:
                    Assembler_to_CargoContainers();
                    break;
                case 3:
                    Refinery_to_CargoContainers();
                    break;
                case 4:
                    Refinery_to_CargoContainers();
                    break;
                case 5:
                    ShowProductionQueue();
                    break;
                case 6:
                    Bottles_to_Tanks("HydrogenBottle", hydrogenTanks);
                    break;
                case 7:
                    Bottles_to_Tanks("OxygenBottle", oxygenTanks);
                    break;
                case 8:
                    RefineriesAutoManager();
                    break;
                case 9:
                    RefineriesAutoManager();
                    break;
                case 10:
                    RefineriesAutoManager();
                    break;
                case 11:
                    AssemblerInputManager();
                    break;
                case 12:
                    AssemblerInputManager();
                    break;
            }
        }


        MyIni _assemblerIni = new MyIni();
        const string assemblerInputManagerSection = "AssemblerInputManager", enable = "enable", defaultEnable = "false"
            , assemblerCustomNameKeyword = "AssemblerCustomNameKeyword", defaultAssemblerCustomNameKeyword = "压缩";
        const string assemblerInputItemListSelection = "InputItemList", assemblerInputItemListLengthKey = "Length";
        Dictionary<string, double> assemblerInputItemTypeIdAmounts = new Dictionary<string, double>();
        
        public void BuildAssemblerInputItemTypeIdAmounts(IMyAssembler assembler) {
            _assemblerIni.Clear();
            assemblerInputItemTypeIdAmounts.Clear();

            if (assembler.CustomData == "0") assembler.CustomData = _assemblerIni.ToString();

            MyIniParseResult result;
            if (!_assemblerIni.TryParse(assembler.CustomData, out result))
                throw new Exception(result.ToString());

            if (!_assemblerIni.ContainsKey(assemblerInputItemListSelection, assemblerInputItemListLengthKey)) {
                _assemblerIni.Set(assemblerInputItemListSelection, assemblerInputItemListLengthKey, 0);
                assembler.CustomData = _assemblerIni.ToString();
                return;
            }

            int length = _assemblerIni.Get(assemblerInputItemListSelection, assemblerInputItemListLengthKey).ToInt32();
            if (length == 0) return;

            for (int i = 1; i <= length; i++)
            {
                string value = _assemblerIni.Get(assemblerInputItemListSelection, i.ToString()).ToString();
                string[] r2 = value.Split(':');

                assemblerInputItemTypeIdAmounts.Add(r2[0], double.Parse(r2[1]));
            }
        }

        public void AssemblerInputManager() {
            Echo("AssemblerInputManager");
            string meEnable = "";
            GetConfiguration_from_CustomData(assemblerInputManagerSection, enable, out meEnable);
            if (meEnable == "")
            {
                meEnable = defaultEnable;
                WriteConfiguration_to_CustomData(assemblerInputManagerSection, enable, meEnable);
            }
            if (meEnable == defaultEnable) return;

            string meAssemblerCustomNameKeyword = "";
            GetConfiguration_from_CustomData(assemblerInputManagerSection, assemblerCustomNameKeyword, out meAssemblerCustomNameKeyword);
            if (meAssemblerCustomNameKeyword == "")
            {
                meAssemblerCustomNameKeyword = defaultAssemblerCustomNameKeyword;
                WriteConfiguration_to_CustomData(assemblerInputManagerSection, assemblerCustomNameKeyword, meAssemblerCustomNameKeyword);
            }

            foreach (IMyAssembler assembler in assemblers.FindAll(ass => ass.CustomName.Contains(defaultAssemblerCustomNameKeyword)).ToList()) {
                BuildAssemblerInputItemTypeIdAmounts(assembler);
                if (assemblerInputItemTypeIdAmounts.Count == 0) continue;

                foreach (var cargoContainer in cargoContainers)
                {
                    if (cargoContainer.DisplayNameText.Contains(CARGO_DISABLE_TAG)) continue;

                    List<MyInventoryItem> items = new List<MyInventoryItem>();
                    cargoContainer.GetInventory().GetItems(items, i => assemblerInputItemTypeIdAmounts.ContainsKey(i.Type.ToString()));
                    foreach (var item in items) {
                        long assemblerInputAmount = assembler.InputInventory.GetItemAmount(item.Type).ToIntSafe();
                        long assemblerInputCapacity = assembler.InputInventory.MaxVolume.ToIntSafe() - assembler.InputInventory.CurrentVolume.ToIntSafe();
                        long cargoItemAmount = item.Amount.ToIntSafe();
                        double setMaxAmount = assemblerInputItemTypeIdAmounts[item.Type.ToString()];
                        if (assemblerInputAmount >= setMaxAmount) continue;
                        long moveAmount = cargoItemAmount - assemblerInputAmount;
                        moveAmount = Math.Min(moveAmount, int.Parse((setMaxAmount - assemblerInputAmount).ToString()));
                        //moveAmount = Math.Min(moveAmount, assemblerInputCapacity);
                        var amount = MyFixedPoint.DeserializeString((moveAmount.ToString()));
                        if (!assembler.InputInventory.CanItemsBeAdded(amount, item.Type)) continue;
                        assembler.InputInventory.TransferItemFrom(cargoContainer.GetInventory(), item, amount);
                    }

                }

            }

        }


        public void Assembler_to_CargoContainers()
        {
            Echo("Assembler_to_CargoContainers");

            for (int i = 0; i < 10; i++)
            {
                int assemblerCounter = counter_AssemblerManagement * 10 + i;

                if (assemblerCounter >= assemblers.Count)
                {
                    counter_AssemblerManagement = 0;
                    return;
                }
                else
                {
                    var assembler = assemblers[assemblerCounter];
                    double currentVolume_Double = ((double)assembler.OutputInventory.CurrentVolume);
                    double maxVolume_Double = ((double)assembler.OutputInventory.MaxVolume);
                    double volumeRatio_Double = currentVolume_Double / maxVolume_Double;

                    if (!assemblers[assemblerCounter].IsProducing)
                    {
                        foreach (var cargoContainer in cargoContainers)
                        {
                            if (cargoContainer.DisplayNameText.Contains(CARGO_DISABLE_TAG)) continue;
                            List<MyInventoryItem> items1 = new List<MyInventoryItem>();
                            assemblers[assemblerCounter].InputInventory.GetItems(items1);
                            foreach (var item in items1)
                            {
                                bool tf = assemblers[assemblerCounter].InputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            }

                            List<MyInventoryItem> items2 = new List<MyInventoryItem>();
                            assemblers[assemblerCounter].OutputInventory.GetItems(items2);
                            foreach (var item in items2)
                            {
                                bool tf = assemblers[assemblerCounter].OutputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            }

                            items1.Clear();
                            items2.Clear();
                            assemblers[assemblerCounter].InputInventory.GetItems(items1);
                            assemblers[assemblerCounter].OutputInventory.GetItems(items2);
                            if (items1.Count < 1 && items2.Count == 0) break;
                        }
                    }
                    else
                    {
                        if (volumeRatio_Double > 0)
                        {
                            foreach (var cargoContainer in cargoContainers)
                            {
                                if (cargoContainer.DisplayNameText.Contains(CARGO_DISABLE_TAG)) continue;
                                List<MyInventoryItem> items3 = new List<MyInventoryItem>();
                                assembler.OutputInventory.GetItems(items3);
                                foreach (var item in items3)
                                {
                                    bool tf = assembler.OutputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                                }
                                items3.Clear();
                                assembler.OutputInventory.GetItems(items3);
                                if (items3.Count < 1) break;
                            }
                        }
                    }
                }
            }

            counter_AssemblerManagement++;
        }// Assembler_to_CargoContainers END!

        public void Refinery_to_CargoContainers()
        {
            Echo("Refinery_to_CargoContainers");
            for (int i = 0; i < 10; i++)
            {
                int refineryCounter = counter_RefineryManagement * 10 + i;

                if (refineryCounter >= refineries.Count)
                {
                    counter_RefineryManagement = 0;
                    return;
                }
                else
                {
                    if (refineries[refineryCounter].IsProducing == false)
                    {
                        foreach (var cargoContainer in cargoContainers)
                        {
                            if (cargoContainer.DisplayNameText.Contains(CARGO_DISABLE_TAG)) continue;
                            List<MyInventoryItem> items1 = new List<MyInventoryItem>();
                            refineries[refineryCounter].InputInventory.GetItems(items1);
                            foreach (var item in items1)
                            {
                                bool tf = refineries[refineryCounter].InputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            }

                            List<MyInventoryItem> items2 = new List<MyInventoryItem>();
                            refineries[refineryCounter].OutputInventory.GetItems(items2);
                            foreach (var item in items2)
                            {
                                bool tf = refineries[refineryCounter].OutputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            }

                            items1.Clear();
                            items2.Clear();
                            refineries[refineryCounter].InputInventory.GetItems(items1);
                            refineries[refineryCounter].OutputInventory.GetItems(items2);
                            if (items1.Count < 1 && items2.Count < 1) break;
                        }
                    }
                    else
                    {
                        foreach (var cargoContainer in cargoContainers)
                        {
                            if (cargoContainer.DisplayNameText.Contains(CARGO_DISABLE_TAG)) continue;
                            //List<MyInventoryItem> items1 = new List<MyInventoryItem>();
                            //refineries[refineryCounter].InputInventory.GetItems(items1);
                            //foreach (var item in items1)
                            //{
                            //    string str = item.Type.ToString();
                            //    if (str.IndexOf("_Ore") != -1)
                            //    {
                            //        bool tf = refineries[refineryCounter].InputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            //    }
                            //}

                            List<MyInventoryItem> items2 = new List<MyInventoryItem>();
                            refineries[refineryCounter].OutputInventory.GetItems(items2);
                            foreach (var item in items2)
                            {
                                bool tf = refineries[refineryCounter].OutputInventory.TransferItemTo(cargoContainer.GetInventory(), item);
                            }
                            items2.Clear();
                            refineries[refineryCounter].OutputInventory.GetItems(items2);
                            if (items2.Count < 1) break;
                        }
                    }
                }
            }
            counter_RefineryManagement++;
        }// Refinery_to_CargoContainers END!


        public void Bottles_to_Tanks(string itemType, List<IMyGasTank> tanks)
        {
            foreach (var tank in tanks)
            {
                if (!tank.AutoRefillBottles) tank.AutoRefillBottles = true;
                foreach (var cargoContainer in cargoContainers)
                {
                    List<MyInventoryItem> items1 = new List<MyInventoryItem>();
                    cargoContainer.GetInventory().GetItems(items1);
                    foreach (var item in items1)
                    {
                        string str = item.Type.ToString();
                        if (str.IndexOf(itemType) != -1)
                        {
                            bool tf = cargoContainer.GetInventory().TransferItemTo(tank.GetInventory(), item);
                        }
                    }
                }
            }
        }// Bottles_to_Tanks END!


        public void ShowProductionQueue()
        {
            StringBuilder str = new StringBuilder();
            foreach (var assembler in assemblers)
            {
                List<MyProductionItem> productionitems = new List<MyProductionItem>();
                assembler.GetQueue(productionitems);
                foreach (var item1 in productionitems)
                {
                    str.Append($"{assembler.CustomName}:{item1.BlueprintId}:{item1.Amount}");
                    str.Append("\n");
                }
            }
            DebugLCD(str.ToString());
        }

        //###############     ManageInventory     ###############



        //###############     Start RefineriesAutoManager     ###############

        const string InputItemListSelection = "InputItemList";
        const string refineriesAutoManagerSelection = "RefineriesAutoManager", notAddWhenInputItemHasValue = "NotAddWhenInputItemHasValue", defaultNotAddWhenInputItemHasValue = "true";
        const string joinHandsInputItemRelationSelection = "JoinHandsInputItemRelation", joinHandsInputItemRelationLength = "Length";
        MyIni _refineryIni = new MyIni();

        /// <summary>
        /// 精炼厂自动管理：定义精炼入料列表
        /// </summary>
        public void RefineriesAutoManager()
        {
            Echo("RefineriesAutoManager");
            foreach (var refinery in refineries)
            {
                RefineryAutoManager(refinery);
            }
        }

        public void RefineryAutoManager(IMyRefinery refinery)
        {
            string customData = refinery.CustomData;

            // This time we _must_ check for failure since the user may have written invalid ini.
            _refineryIni.Clear();
            MyIniParseResult result;
            if (!_refineryIni.TryParse(customData, out result))
                throw new Exception(result.ToString());

            // 是否有输入列表配置，如没有则添加
            if (!_refineryIni.ContainsKey(refineriesAutoManagerSelection, notAddWhenInputItemHasValue)) _refineryIni.Set(refineriesAutoManagerSelection, notAddWhenInputItemHasValue, defaultNotAddWhenInputItemHasValue);

            // 关闭耗尽
            if (refinery.UseConveyorSystem) refinery.UseConveyorSystem = false; 
            
            // 是否有联合精炼配置，如没有则添加
            if (!_refineryIni.ContainsKey(joinHandsInputItemRelationSelection, joinHandsInputItemRelationLength)) _refineryIni.Set(joinHandsInputItemRelationSelection, joinHandsInputItemRelationLength, 0);
            List<string[]> joinHandsItemRelations = new List<string[]>();
            int joinHandsItemRelationLengthInt = _refineryIni.Get(joinHandsInputItemRelationSelection, joinHandsInputItemRelationLength).ToInt32();
            for (int i = 1; i <= joinHandsItemRelationLengthInt; i++) { 
                joinHandsItemRelations.Add(_refineryIni.Get(joinHandsInputItemRelationSelection, i.ToString()).ToString().Split(':'));
            }

            long residualVolume = refinery.InputInventory.MaxVolume.RawValue - refinery.InputInventory.CurrentVolume.RawValue;

            foreach (var cargo in cargoContainers) {
                List<MyInventoryItem> items = new List<MyInventoryItem>();
                cargo.GetInventory().GetItems(items, (i) => i.Type.ToString().StartsWith("MyObjectBuilder_Ore"));
                foreach (var item in items)
                {
                    string typeStr = item.Type.ToString();
                    string key = typeStr.Replace('/', '_');
                    string chTypeStr = TranslateName(typeStr);

                    if (!cargo.GetInventory().CanTransferItemTo(refinery.InputInventory, item.Type)) continue;

                    if (!_refineryIni.ContainsKey(InputItemListSelection, key)) _refineryIni.Set(InputItemListSelection, key, chTypeStr + ":" + Math.Min(10000, residualVolume).ToString());

                    var strs = _refineryIni.Get(InputItemListSelection, key).ToString().Split(':');
                    var defineValue = long.Parse(strs[1]);
                    if (defineValue <= 100) continue;

                    string[] needJoinHandsItemRealtion = { };
                    foreach (var joinHandsItemRelation in joinHandsItemRelations) {
                        foreach (var typeStr2 in joinHandsItemRelation) {
                            if (typeStr2 == typeStr) {
                                needJoinHandsItemRealtion = joinHandsItemRelation;
                                break;
                            }
                        }
                        if (needJoinHandsItemRealtion.Length > 0) { break; }
                    }

                    // 校验箱子里的联合精炼列表里的物品数目都大于0，如果有一项小于0，则不移动该原料物品
                    bool needMove = true;
                    foreach (var needCheckTypeStr in needJoinHandsItemRealtion) {
                        foreach (var c2 in cargoContainers) {
                            int amount = c2.GetInventory().GetItemAmount(MyItemType.Parse(needCheckTypeStr)).ToIntSafe();
                            if (amount <= 0) { 
                                needMove = false; 
                                break;
                            }
                        }
                        if (!needMove) { break; }
                    }
                    if (!needMove) continue;


                    var rItem = refinery.InputInventory.FindItem(item.Type);

                    if (rItem.HasValue && _refineryIni.Get(refineriesAutoManagerSelection, notAddWhenInputItemHasValue).ToBoolean()) continue;
                    
                    long rItemAmount = 0;
                    if (rItem.HasValue) rItemAmount = rItem.Value.Amount.ToIntSafe();
                    if (rItemAmount >= defineValue) continue;

                    long addAmount = defineValue - rItemAmount;

                    addAmount = Math.Min(addAmount, residualVolume);
                    addAmount = Math.Min(addAmount, item.Amount.ToIntSafe());

                    refinery.InputInventory.TransferItemFrom(cargo.GetInventory(), item, MyFixedPoint.DeserializeStringSafe(addAmount.ToString()));
                }

            }

            refinery.CustomData = _refineryIni.ToString();
        }

        //###############     End RefineriesAutoManager     ###############

        public void ProgrammableBlockScreen()
        {

            //  512 X 320
            IMyTextSurface panel = Me.GetSurface(0);

            if (panel == null) return;
            panel.ContentType = ContentType.SCRIPT;

            MySpriteDrawFrame frame = panel.DrawFrame();

            float x = 512 / 2, y1 = 205;
            DrawLogo(frame, x, y1, 200);
            PanelWriteText(frame, "Base_Inventory_Management\nWith_Graphic_Interface_V2.0\nby Hi.James and Chivehao.", x, y1 + 110, 1f, TextAlignment.CENTER);

            frame.Dispose();

        }


        //###############     Start CargoAutoManager     ###############

        string cargoAutoManagerSelection = "CargoAutoManager", 
            enableCargoAutoManagerKey = "enable", defaultEnableCargoAutoManager = "false";
        // 存放矿物的货箱标签
        string CARGO_ORE_TAG = "[矿物]";
        // 存放锭的货箱标签
        string CARGO_INGOT_TAG = "[矿锭]";
        // 存放组件的货箱标签
        string CARGO_COMPONENT_TAG = "[零件]";
        // 存放工具的货箱标签
        string CARGO_TOOL_TAG = "[工具]";
        // 存放弹药的货箱标签
        string CARGO_AMMO_TAG = "[弹药]";
        // 不计入统计的货箱标签
        string CARGO_DISABLE_TAG = "[禁止]";
        string[] filterBlockNames = {
            "MyObjectBuilder_OxygenGenerator",
            "MyObjectBuilder_Reactor",
            "MyObjectBuilder_LargeMissileTurret",
            "MyObjectBuilder_LargeGatlingTurret",
            "MyObjectBuilder_InteriorTurret",
            "MyObjectBuilder_TurretControlBlock",
            "MyObjectBuilder_OxygenTank"
        };
        string[] typeIdTools = {
            "MyObjectBuilder_PhysicalGunObject",
            "MyObjectBuilder_GasContainerObject",
            "MyObjectBuilder_OxygenContainerObject"
        };
        /// <summary>
        /// 箱子自动整理
        /// </summary>
        public void CargoAutoManager() {
            if (!_ini.ContainsKey(cargoAutoManagerSelection, enableCargoAutoManagerKey))
            {
                _ini.Set(cargoAutoManagerSelection, enableCargoAutoManagerKey, defaultEnableCargoAutoManager);
                Me.CustomData = _ini.ToString();
            }
            if (!_ini.Get(cargoAutoManagerSelection, enableCargoAutoManagerKey).ToBoolean()) return;


            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.HasInventory && !filterBlockNames.Contains(block.BlockDefinition.TypeIdString) && !block.DisplayNameText.Contains(CARGO_DISABLE_TAG));
            Echo($"有效货箱数:{blocks.Count}");
            //矿物箱子
            var oreBlock = blocks.Where(d => d.DisplayNameText.Contains(CARGO_ORE_TAG)).ToList();
            //锭箱子
            var ingotBlock = blocks.Where(d => d.DisplayNameText.Contains(CARGO_INGOT_TAG)).ToList();
            //组件箱子
            var componentBlock = blocks.Where(d => d.DisplayNameText.Contains(CARGO_COMPONENT_TAG)).ToList();
            //工具箱子
            var toolBlock = blocks.Where(d => d.DisplayNameText.Contains(CARGO_TOOL_TAG)).ToList();
            //弹药箱子
            var ammoBlock = blocks.Where(d => d.DisplayNameText.Contains(CARGO_AMMO_TAG)).ToList();

            IEnumerable<String> oreBlockNameList = oreBlock.Select(d => d.Name);
            IEnumerable<String> ingotBlockNameList = ingotBlock.Select(d => d.Name);
            IEnumerable<String> componentBlockNameList = componentBlock.Select(d => d.Name);
            IEnumerable<String> toolBlockNameList = toolBlock.Select(d => d.Name);
            IEnumerable<String> ammoBlockNameList = ammoBlock.Select(d => d.Name);

            foreach (IMyTerminalBlock block in blocks)
            {
                // Echo($"货箱:{block.DisplayNameText}");
                IMyInventory currentInventory = null;
                if (block.InventoryCount > 1)
                {
                    // 精炼机和组装机取输出容器
                    currentInventory = block.GetInventory(1);
                }
                else
                {
                    // 普通方块取默认容器
                    currentInventory = block.GetInventory();
                }
                if (currentInventory.CurrentVolume.RawValue <= 0L)
                {
                    continue;
                }
                List<MyInventoryItem> oreItems = getItem(block, oreBlockNameList, currentInventory, "MyObjectBuilder_Ore");
                List<MyInventoryItem> ingotItems = getItem(block, ingotBlockNameList, currentInventory, "MyObjectBuilder_Ingot");
                List<MyInventoryItem> componentItems = getItem(block, componentBlockNameList, currentInventory, "MyObjectBuilder_Component");
                List<MyInventoryItem> toolItems = getItem(block, toolBlockNameList, currentInventory, null, typeIdTools);
                List<MyInventoryItem> ammoItems = getItem(block, ammoBlockNameList, currentInventory, "MyObjectBuilder_AmmoMagazine");

                TransferItem(oreItems, block, currentInventory, oreBlock);
                TransferItem(ingotItems, block, currentInventory, ingotBlock);
                TransferItem(componentItems, block, currentInventory, componentBlock);
                TransferItem(toolItems, block, currentInventory, toolBlock);
                TransferItem(ammoItems, block, currentInventory, ammoBlock);
            }

        }

        // 获取物品
        List<MyInventoryItem> getItem(IMyTerminalBlock block, IEnumerable<String> nameList, IMyInventory inventory, string typeId, string[] typeIds = null)
        {
            List<MyInventoryItem> list = new List<MyInventoryItem>();
            if (!nameList.Contains(block.Name))
            {
                if (typeId != null)
                {
                    inventory.GetItems(list, d => d.Type.TypeId == typeId);
                }
                else
                {
                    inventory.GetItems(list, d => typeIds.Contains(d.Type.TypeId));
                }
            }
            return list;
        }

        // 转移物品
        void TransferItem(List<MyInventoryItem> items, IMyTerminalBlock fromBlock, IMyInventory currentInventory, List<IMyTerminalBlock> toBlocks)
        {
            if (items.Count == 0)
            {
                return;
            }
            foreach (IMyTerminalBlock toBlock in toBlocks)
            {
                if (toBlock.Name == fromBlock.Name)
                {
                    continue;
                }
                IMyInventory inventory = toBlock.GetInventory();
                foreach (MyInventoryItem item in items)
                {
                    if (inventory.IsFull)
                    {
                        break;
                    }
                    currentInventory.TransferItemTo(inventory, item);
                }
            }
        }


        //###############     End CargoAutoManager     ###############

        private int statsTimeInterval = 5; // s
        private DateTime lastTime;
        public void UpdateItemStatsList() {
            TimeSpan elapsedTime = DateTime.Now - lastTime;
            if (elapsedTime.TotalSeconds < statsTimeInterval) return;

            lastTime = DateTime.Now;

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.HasInventory && !filterBlockNames.Contains(block.BlockDefinition.TypeIdString) && !block.DisplayNameText.Contains(CARGO_DISABLE_TAG));

            Dictionary<string, long> statsMap = new Dictionary<string, long>();

            foreach (var type in statsItemTyps) {
                statsMap.Add(type, 0);
            }

            foreach (var block in blocks) {
                IMyInventory currentInventory = null;
                if (block.InventoryCount > 1)
                {
                    // 精炼机和组装机取输出容器
                    currentInventory = block.GetInventory(1);
                }
                else
                {
                    // 普通方块取默认容器
                    currentInventory = block.GetInventory();
                }
                if (currentInventory.CurrentVolume.RawValue <= 0L)
                {
                    continue;
                }

                foreach (var type in statsItemTyps)
                {
                    var count = currentInventory.GetItemAmount(MyItemType.Parse(type)).RawValue / 1000000;
                    statsMap[type] += count;

                }

            }


            foreach (var name in statsItemTyps)
            {
                int index = itemStatsList.FindIndex(e => e.Name == name);
                long count = statsMap[name];
                if (index == -1)
                {
                    ItemStats newStats = new ItemStats();
                    newStats.Name = name;
                    newStats.Difference = 0;
                    newStats.StartTime = DateTime.Now;
                    newStats.Count = count;
                    newStats.LastCount = count;
                    itemStatsList.Add(newStats);
                }
                else
                {
                    ItemStats stats = itemStatsList[index];
                    stats.LastCount = stats.Count;
                    stats.Count = count;
                    stats.Difference = stats.Count - stats.LastCount;

                    itemStatsList.RemoveAt(index);
                    itemStatsList.Add(stats);

                }
            }

            RenderStatisticsPanel(statisticsPanel, itemStatsList);
            RenderStatisticsPanels();
        }



        public void RenderStatisticsPanel(IMyTextPanel panel, List<ItemStats> stats) { 
            if (panel == null || stats == null || stats.Count == 0) { return; }
            var frame = panel.DrawFrame();
            panel.ContentType = ContentType.SCRIPT;
            for (int i = 0; i < Math.Min(20, stats.Count); i++)
            {
                RenderLcdItemStatsUnit(i, frame, stats[i]);
            }
            frame.Dispose();
        }

        public void RenderStatisticsPanels() { 
            if (statisticsPanels == null ||  statisticsPanels.Count == 0) { return; }
            foreach (var panel in statisticsPanels) {
                var names = panel.CustomName.Split(':');
                if (names.Length != 2) { continue; }
                var index = Convert.ToInt32(names[1]);
                int start = 20 * (index - 1);
                int end = start + Math.Min(19, itemStatsList.Count - 1 - start);
                List<ItemStats> newStats = itemStatsList.GetRange(start, end - start + 1);
                RenderStatisticsPanel(panel, newStats);
            }

        }

        public string ConvertTimeStr(long totalSeconds) {

            long days = totalSeconds / (24 * 3600);  // 计算天数
            long hours = (totalSeconds % (24 * 3600)) / 3600;  // 计算小时
            long minutes = (totalSeconds % 3600) / 60;  // 计算分钟
            long seconds = totalSeconds % 60;  // 计算秒数

            string formattedTime = "";
            if (days > 0)
                return $"{days}天{hours}时";
            if (hours > 0)
                return $"{hours}小时{minutes}分";
            if (minutes > 0)
                return $"{minutes}分钟{seconds}秒";
            if (seconds > 0 || formattedTime == "") // 如果秒数大于0，或者没有显示任何时间单位，则显示秒
                return $"{seconds}秒";

            return $"{seconds}秒";
        }

        public void RenderTestPanel() {
            if (testPanel == null) return;
            //testPanel.ContentType = ContentType.SCRIPT;
            //var frame = testPanel.DrawFrame();
            //for (int i = 0; i < itemStatsList.Count; i++) {
            //    RenderLcdItemStatsUnit(i, frame, itemStatsList[i]);
            //}
            //frame.Dispose();
        }

        float lineHeight = 25.5F;
        public void RenderLcdItemStatsUnit(int index, MySpriteDrawFrame frame, ItemStats itemStats)
        {
            //  ItemAmount box
            float h = lineHeight;
            float width = 150f;
            float x1 = width / 2;
            float y1 = lineHeight / 2 + lineHeight * index;
            float textY = y1 - lineHeight / 2 + 2F, textSize = 0.75f;
            DrawBox(frame, x1, y1, width, lineHeight, background_Color);
            PanelWriteText(frame, itemStats.Count.ToString("N0"), x1 + width / 2 - 2f, textY, textSize, TextAlignment.RIGHT);

            //  picture box
            float x2 = x1 + width / 2 + lineHeight / 2 + 0.5f;
            float boxWH_Float = lineHeight - 1;
            DrawBox(frame, x2, y1, lineHeight, lineHeight, background_Color);
            MySprite sprite = MySprite.CreateSprite(itemStats.Name, new Vector2(x2, y1), new Vector2(boxWH_Float, boxWH_Float));
            frame.Add(sprite);

            // name box
            float nameWidth = 100f;
            float x3 = x2 + nameWidth / 2 + lineHeight / 2 + 0.5f;
            DrawBox(frame, x3, y1, nameWidth, lineHeight, background_Color);
            string name = TranslateName(itemStats.Name);
            if (name.Length > 7) name = name.Substring(0, 6) + "...";
            PanelWriteText(frame, name, x2 + lineHeight / 2 + 2f, textY, textSize, TextAlignment.LEFT);

            // up or down box
            float x4 = x3 + nameWidth / 2 + lineHeight / 2 + 0.5f;
            DrawBox(frame, x4, y1, lineHeight, lineHeight, background_Color);
            string spriteName = "LCD_Emote_Neutral";
            Color color = Color.White;
            if (itemStats.Difference > 0) {
                spriteName = "LCD_Emote_Happy";
                color = new Color(0, 140, 0);
            }
            if (itemStats.Difference < 0) {
                spriteName = "LCD_Emote_Sad";
                color = new Color(130, 100, 0);
            }
            sprite = MySprite.CreateSprite(spriteName, new Vector2(x4, y1), new Vector2(boxWH_Float - 4, boxWH_Float - 4));
            sprite.Color = color;
            frame.Add(sprite);
            sprite.Color = Color.White;

            // speed box
            float speedBoxWidth = 100f;
            float x5 = x4 + speedBoxWidth / 2 + lineHeight / 2 + 0.5f;
            DrawBox(frame, x5, y1, speedBoxWidth, lineHeight, background_Color);
            var difference = itemStats.Difference;
            var differenceStr = "";
            if (Math.Abs(difference) > 1000000) differenceStr = (difference / 1000000).ToString("N0") + "M";
            else if(Math.Abs(difference) > 1000) differenceStr = (difference / 1000).ToString("N0") + "K";
            else differenceStr = difference.ToString("N0");
            PanelWriteText(frame, differenceStr + " /" + statsTimeInterval + "秒", x5 - speedBoxWidth / 2 + 2f, textY, textSize, TextAlignment.LEFT);

            // 预计消耗时间盒子
            float tiemBoxwidth = 112f;
            float x6 = x4 + lineHeight / 2 + speedBoxWidth + tiemBoxwidth / 2 + 0.5f;
            DrawBox(frame, x6, y1, tiemBoxwidth, lineHeight, background_Color);
            if (itemStats.Difference < 0 && itemStats.Count > 10)
            {
                var speed = Math.Abs(itemStats.Difference) / Convert.ToInt64(statsTimeInterval);
                var time = itemStats.Count / speed;
                string timeStr = ConvertTimeStr(Convert.ToInt64(time));
                PanelWriteText(frame, timeStr, x6 - tiemBoxwidth / 2 + 2f, textY, textSize, TextAlignment.LEFT);
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Echo($"{DateTime.Now}");
            Echo("Program is running.");

            RenderTestPanel();

            if (argument == "ReloadBlocksFromGridTerminalSystem" || argument == "ReloadBlocksFromGridTerminalSystem1" || argument == "ReloadBlocksFromGridTerminalSystem2")
            {
                // DebugLCD("ReloadBlocksFromGridTerminalSystem: argument=" + argument);
                GetBlocksFromGridTerminalSystem();
            }

            if (counter_ProgramRefresh != 1 || counter_ProgramRefresh != 6 || counter_ProgramRefresh != 11 || counter_ProgramRefresh != 16)
            {
                if (counter_Logo++ >= 360) counter_Logo = 0;

                ProgrammableBlockScreen();

                OverallDisplay();

                UpdateItemStatsList();
            }

            if (counter_ProgramRefresh++ >= 21) counter_ProgramRefresh = 0;

            switch (counter_ProgramRefresh)
            {
                case 1:
                    Echo("ShowItems");
                    ShowItems();
                    break;
                case 6:
                    Echo("ShowFacilities");
                    ShowFacilities();
                    break;
                case 11:
                    Echo("ManageInventory");
                    ManageInventory();
                    break;
                case 16:
                    Echo("CargoAutoManager");
                    CargoAutoManager();
                    break;
            }



        }
    }
}



