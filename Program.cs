using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        MyIni _ini = new MyIni();

        List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
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
        List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
        List<IMyReactor> reactors = new List<IMyReactor>();
        List<IMyGasGenerator> gasGenerators = new List<IMyGasGenerator>();
        List<string> spritesList = new List<string>();


        Dictionary<string, string> translator = new Dictionary<string, string>();
        Dictionary<string, double> productionList = new Dictionary<string, double>();

        const int itemAmountInEachScreen = 35, facilityAmountInEachScreen = 20;
        const float itemBox_ColumnInterval_Float = 73, itemBox_RowInterval_Float = 102, amountBox_Height_Float = 24, facilityBox_RowInterval_Float = 25.5f;
        const string information_Section = "Information", broadCastConnectorGPS_Key = "BroadCastConnectorGPS (Y/N)";
        const string translateList_Section = "Translate_List", length_Key = "Length", autoProduction_Section = "Auto_Production", autoProduction_Key = "Auto_Production(Y/N)", autoPowerControl_Key = "Auto_Power_Control(Y/N)";
        int counter_ProgramRefresh = 0, counter_ShowItems = 0, counter_ShowFacilities = 0, counter_InventoryManagement = 0, counter_AssemblerManagement = 0, counter_RefineryManagement = 0, counter_Panel = 0;
        double counter_Logo = 0;
        const string icetoUranium_Section = "Ice_To_Uranium", buttonOn_Key = "Button_On";

        Color background_Color = new Color(0, 35, 45);
        Color border_Color = new Color(0, 130, 255);

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


        public struct ComparisonTable
        {
            public string Name;
            public string BluePrintName;
            public double Amount;
            public bool HasItem;
        }

        ComparisonTable[] ComparisonTable_AutoProduction;

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

            BuildProductionListBase();

            GridTerminalSystem.GetBlocksOfType(cargoContainers, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(panels, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(panels_Overall, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Overall_Display"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_All, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Ore, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Ore_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Ingot, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Ingot_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_Component, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Component_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Items_AmmoMagazine, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_AmmoMagazine_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Refineries, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Refinery_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(panels_Assemblers, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("LCD_Assembler_Inventory_Display:"));
            GridTerminalSystem.GetBlocksOfType(oxygenTanks, b => b.IsSameConstructAs(Me) && !b.DefinitionDisplayNameText.ToString().Contains("Hydrogen") && !b.DefinitionDisplayNameText.ToString().Contains("氢气"));
            GridTerminalSystem.GetBlocksOfType(hydrogenTanks, b => b.IsSameConstructAs(Me) && !b.DefinitionDisplayNameText.ToString().Contains("Oxygen") && !b.DefinitionDisplayNameText.ToString().Contains("氧气"));
            GridTerminalSystem.GetBlocksOfType(assemblers, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(refineries, b => b.IsSameConstructAs(Me) && !b.BlockDefinition.ToString().Contains("Shield"));
            GridTerminalSystem.GetBlocksOfType(powerProducers); // 连接器连接的网格的发电设备也算进来
            GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(reactors, b => b.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(gasGenerators, b => b.IsSameConstructAs(Me));

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

            foreach (var assembler in assemblers) assembler.CustomData = "0";

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

            // This time we _must_ check for failure since the user may have written invalid ini.
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
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
                _ini.Set(information_Section, broadCastConnectorGPS_Key, "N");
                _ini.Set(information_Section, "LCD_Overall_Display", "LCD_Overall_Display | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Inventory_Display", "LCD_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Ore_Inventory_Display", "LCD_Ore_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Ingot_Inventory_Display", "LCD_Ingot_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Component_Inventory_Display", "LCD_Component_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_AmmoMagazine_Inventory_Display", "LCD_AmmoMagazine_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Refinery_Inventory_Display", "LCD_Refinery_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "LCD_Assembler_Inventory_Display", "LCD_Assembler_Inventory_Display:X | X=1,2,3... | Fill In CustomName of Panel");
                _ini.Set(information_Section, "Check_Cockpit", "CPT | Fill In Argument of Programmable Block And Press Run");
                _ini.Set(information_Section, "Assemblers_CooperativeMode", "CO_ON or CO_OFF | Fill In Argument of Programmable Block And Press Run");
                _ini.Set(information_Section, "Assemblers_On", "ASS_ON:X | X=1,2,3... | Fill In Argument of Programmable Block And Press Run");
                _ini.Set(information_Section, "Refineries_On", "REF_ON:X | X=1,2,3... | Fill In Argument of Programmable Block And Press Run");
                _ini.Set(translateList_Section, length_Key, "1");
                _ini.Set(translateList_Section, "1", "AH_BoreSight:More");

                _ini.Set(autoProduction_Section, autoPowerControl_Key, "N");
                _ini.Set(autoProduction_Section, autoProduction_Key, "N");
                _ini.Set(autoProduction_Section, length_Key, "21");
                _ini.Set(autoProduction_Section, "1", "MyObjectBuilder_Component/SteelPlate:MyObjectBuilder_BlueprintDefinition/SteelPlate:5000");
                _ini.Set(autoProduction_Section, "2", "MyObjectBuilder_Component/InteriorPlate:MyObjectBuilder_BlueprintDefinition/InteriorPlate:5000");
                _ini.Set(autoProduction_Section, "3", "MyObjectBuilder_Component/Construction:MyObjectBuilder_BlueprintDefinition/ConstructionComponent:5000");
                _ini.Set(autoProduction_Section, "4", "MyObjectBuilder_Component/Motor:MyObjectBuilder_BlueprintDefinition/MotorComponent:5000");
                _ini.Set(autoProduction_Section, "5", "MyObjectBuilder_Component/Computer:MyObjectBuilder_BlueprintDefinition/ComputerComponent:5000");
                _ini.Set(autoProduction_Section, "6", "MyObjectBuilder_Component/MetalGrid:MyObjectBuilder_BlueprintDefinition/MetalGrid:5000");
                _ini.Set(autoProduction_Section, "7", "MyObjectBuilder_Component/SmallTube:MyObjectBuilder_BlueprintDefinition/SmallTube:5000");
                _ini.Set(autoProduction_Section, "8", "MyObjectBuilder_Component/LargeTube:MyObjectBuilder_BlueprintDefinition/LargeTube:5000");
                _ini.Set(autoProduction_Section, "9", "MyObjectBuilder_Component/Display:MyObjectBuilder_BlueprintDefinition/Display:5000");
                _ini.Set(autoProduction_Section, "10", "MyObjectBuilder_Component/Girder:MyObjectBuilder_BlueprintDefinition/GirderComponent:5000");
                _ini.Set(autoProduction_Section, "11", "MyObjectBuilder_Component/BulletproofGlass:MyObjectBuilder_BlueprintDefinition/BulletproofGlass:5000");
                _ini.Set(autoProduction_Section, "12", "MyObjectBuilder_Component/Detector:MyObjectBuilder_BlueprintDefinition/DetectorComponent:5000");
                _ini.Set(autoProduction_Section, "13", "MyObjectBuilder_Component/Reactor:MyObjectBuilder_BlueprintDefinition/ReactorComponent:5000");
                _ini.Set(autoProduction_Section, "14", "MyObjectBuilder_Component/PowerCell:MyObjectBuilder_BlueprintDefinition/PowerCell:5000");
                _ini.Set(autoProduction_Section, "15", "MyObjectBuilder_Component/SolarCell:MyObjectBuilder_BlueprintDefinition/SolarCell:5000");
                _ini.Set(autoProduction_Section, "16", "MyObjectBuilder_Component/Superconductor:MyObjectBuilder_BlueprintDefinition/Superconductor:5000");
                _ini.Set(autoProduction_Section, "17", "MyObjectBuilder_Component/GravityGenerator:MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent:100");
                _ini.Set(autoProduction_Section, "18", "MyObjectBuilder_Component/Medical:MyObjectBuilder_BlueprintDefinition/MedicalComponent:100");
                _ini.Set(autoProduction_Section, "19", "MyObjectBuilder_Component/RadioCommunication:MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent:100");
                _ini.Set(autoProduction_Section, "20", "MyObjectBuilder_GasContainerObject/HydrogenBottle:MyObjectBuilder_BlueprintDefinition/Position0020_HydrogenBottle:100");
                _ini.Set(autoProduction_Section, "21", "MyObjectBuilder_OxygenContainerObject/OxygenBottle:MyObjectBuilder_BlueprintDefinition/Position0010_OxygenBottle:100");
                Me.CustomData = _ini.ToString();
            }// End if

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

            // Battery
            float y5 = y4 + itemBox_ColumnInterval_Float;
            sprite = MySprite.CreateSprite("MyObjectBuilder_Component/PowerCell", new Vector2(x_Left, y5), new Vector2(itemBox_ColumnInterval_Float - 2, itemBox_ColumnInterval_Float - 2));
            frame.Add(sprite);
            CalculateBatteries(out percentage_String, out finalValue_String);
            PanelWriteText(frame, batteries.Count.ToString(), x_Title, y_Title + itemBox_ColumnInterval_Float * 4, 0.55f, TextAlignment.RIGHT);
            ProgressBar(frame, x_Right, y5 + progressBar_YCorrect, progressBarWidth, progressBarHeight, percentage_String);
            PanelWriteText(frame, percentage_String, x_Right, y_Title + itemBox_ColumnInterval_Float * 4, 1.2f, TextAlignment.CENTER);
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


        public void CalculateBatteries(out string percentage_String, out string finalValue_String)
        {
            double currentOutput = 0, totalOutput = 0;
            foreach (var battery in batteries)
            {
                currentOutput += battery.CurrentStoredPower;
                totalOutput += battery.MaxStoredPower;
            }

            percentage_String = Math.Round(currentOutput / totalOutput * 100, 1).ToString() + "%";
            finalValue_String = AmountUnitConversion(currentOutput * 1000000) + " W / " + AmountUnitConversion(totalOutput * 1000000) + " W";
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

                panel.WriteText($"{(k + 1).ToString() + ".\n"}{facilityList[k].Name}", true);
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
            if (isEnabled)
            {
                if (isProducing) DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(0, 140, 0));
                else DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(130, 100, 0));
            }
            else
            {
                DrawBox(frame, x3, y1, boxWH_Float, boxWH_Float, new Color(178, 9, 9));
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

            string temp = "Textures\\FactionLogo\\Empty.dds";
            foreach (var sprite in spritesList)
            {
                if (sprite.IndexOf(blueprintIds[blueprintIds.Length - 1]) != -1)
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
            if (counter_InventoryManagement++ >= 11) counter_InventoryManagement = 1;

            switch (counter_InventoryManagement)
            {
                case 1:
                    Assembler_to_CargoContainers();
                    break;
                case 2:
                    Refinery_to_CargoContainers();
                    break;
                case 3:
                    Assembler_to_CargoContainers();
                    break;
                case 4:
                    ShowProductionQueue();
                    break;
                case 5:
                    Assembler_to_CargoContainers();
                    break;
                case 6:
                    Bottles_to_Tanks("HydrogenBottle", hydrogenTanks);
                    break;
                case 7:
                    Assembler_to_CargoContainers();
                    break;
                case 8:
                    Bottles_to_Tanks("OxygenBottle", oxygenTanks);
                    break;
                case 9:
                    Assembler_to_CargoContainers();
                    break;
                case 10:
                    PowerPlanRefineries();
                    PowerPlanAssemblers();
                    break;
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

                    if (assemblers[assemblerCounter].IsProducing == false)
                    {
                        foreach (var cargoContainer in cargoContainers)
                        {
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


        /*###############     FacilitiesControl     ###############*/
        public void FacilitiesControl(string argument)
        {
            switch (argument)
            {
                case "CO_ON":
                    CooperativeMode(true);
                    break;
                case "CO_OFF":
                    CooperativeMode(false);
                    break;
            }

            Assemblers(argument);
            Refineries(argument);
        }

        public void CooperativeMode(bool true_false)
        {
            foreach (var assembler in assemblers)
            {
                if (true_false && !assembler.CooperativeMode) assembler.CooperativeMode = true;
                if (!true_false && assembler.CooperativeMode) assembler.CooperativeMode = false;
            }
        }

        public void Assemblers(string argument)
        {
            if (argument.Contains("ASS_ON:"))
            {
                string[] str = argument.Split(':');
                double facilityPercentage = 0;

                if (str.Length < 2) facilityPercentage = 100;
                else if (str[1] == "") facilityPercentage = 100;
                else if (Convert.ToDouble(str[1]) > 100) facilityPercentage = 100;
                else if (Convert.ToDouble(str[1]) < 0) facilityPercentage = 0;
                else facilityPercentage = Convert.ToDouble(str[1]);

                double facilitynumber = facilityPercentage * assemblers.Count / 100;

                if (facilitynumber < 1) facilitynumber = 1;

                foreach (var assembler in assemblers) if (assembler.Enabled) assembler.Enabled = false;

                int k = 1;
                foreach (var assembler in assemblers)
                {
                    if (k > facilitynumber)
                    {
                        return;
                    }
                    else
                    {
                        if (!assembler.Enabled) assembler.Enabled = true;
                    }
                    k++;
                }
            }
        }

        public void Refineries(string argument)
        {
            if (argument.Contains("REF_ON:"))
            {
                string[] str = argument.Split(':');
                double facilityPercentage = 0;

                if (str.Length < 2) facilityPercentage = 100;
                else if (str[1] == "") facilityPercentage = 100;
                else if (Convert.ToDouble(str[1]) > 100) facilityPercentage = 100;
                else if (Convert.ToDouble(str[1]) < 0) facilityPercentage = 0;
                else facilityPercentage = Convert.ToDouble(str[1]);

                double facilitynumber = facilityPercentage * refineries.Count / 100;

                if (facilitynumber < 1) facilitynumber = 1;

                foreach (var refinery in refineries) if (refinery.Enabled) refinery.Enabled = false;

                int k = 1;
                foreach (var refinery in refineries)
                {
                    if (k > facilitynumber)
                    {
                        return;
                    }
                    else
                    {
                        if (!refinery.Enabled) refinery.Enabled = true;
                    }
                    k++;
                }
            }
        }

        public void PowerPlanRefineries()
        {
            double oreAmount_Double = 0;
            foreach (var item in itemList_Ore)
            {
                if (item.Name.IndexOf("MyObjectBuilder_Ore/Ice") == -1) oreAmount_Double += item.Amount;
            }

            if (oreAmount_Double == 0)
            {
                foreach (var refinery in refineries) refinery.Enabled = false;
            }
            else
            {
                foreach (var refinery in refineries) refinery.Enabled = true;
            }
        }

        public void PowerPlanAssemblers()
        {
            string autoProduction;
            GetConfiguration_from_CustomData(autoProduction_Section, autoProduction_Key, out autoProduction);

            if (autoProduction == "Y")
            {
                CompareAmount();
                AddtoProductionQueue();
            }
        }

        public void BuildProductionListBase()
        {
            string length_String;
            GetConfiguration_from_CustomData(autoProduction_Section, length_Key, out length_String);

            ComparisonTable_AutoProduction = new ComparisonTable[Convert.ToInt16(length_String)];

            for (int i = 0; i < Convert.ToInt16(length_String); i++)
            {
                string temp;
                GetConfiguration_from_CustomData(autoProduction_Section, (i + 1).ToString(), out temp);
                ParseProductionList(temp, out ComparisonTable_AutoProduction[i].Name, out ComparisonTable_AutoProduction[i].BluePrintName, out ComparisonTable_AutoProduction[i].Amount);
            }
        }

        public void ParseProductionList(string context, out string name, out string bluePrintName, out double amount)
        {
            string[] arry = context.Split(':');
            name = arry[0];
            bluePrintName = arry[1];
            amount = Convert.ToDouble(arry[2]);
        }

        public void CompareAmount()
        {
            productionList.Clear();
            for (int k = 0; k < ComparisonTable_AutoProduction.Length; k++)
            {
                ComparisonTable_AutoProduction[k].HasItem = false;
                foreach (var item in itemList_Component)
                {
                    if (item.Name == ComparisonTable_AutoProduction[k].Name && ComparisonTable_AutoProduction[k].Amount != 0)
                    {
                        ComparisonTable_AutoProduction[k].HasItem = true;
                        if (item.Amount < ComparisonTable_AutoProduction[k].Amount * 1000000)
                        {
                            productionList.Add(ComparisonTable_AutoProduction[k].BluePrintName, ComparisonTable_AutoProduction[k].Amount / 2);
                        }
                        break;
                    }
                }
                if (ComparisonTable_AutoProduction[k].HasItem == false && ComparisonTable_AutoProduction[k].Amount != 0)
                {
                    productionList.Add(ComparisonTable_AutoProduction[k].BluePrintName, ComparisonTable_AutoProduction[k].Amount / 2);
                }
            }
        }

        public void AddtoProductionQueue()
        {
            string autoPowerControl;
            GetConfiguration_from_CustomData(autoProduction_Section, autoPowerControl_Key, out autoPowerControl);

            Echo("PowerControl");
            if (productionList.Count < 1 && autoPowerControl == "Y")
            {
                foreach (var assembler in assemblers)
                {
                    if (!assembler.IsProducing)
                    {
                        if (Convert.ToInt16(assembler.CustomData) > 20)
                        {
                            assembler.Enabled = false;
                            assembler.CustomData = "0";
                        }
                        else
                        {
                            assembler.CustomData = (Convert.ToInt16(assembler.CustomData) + 1).ToString();
                        }
                    }
                }
                return;
            }

            Echo("AddQueue");
            foreach (var assembler in assemblers)
            {
                Echo(assembler.CustomName.ToString());
                bool canAdd = false;

                if (assembler.Enabled == true)
                {
                    if (!assembler.IsProducing && assembler.IsQueueEmpty && !assembler.CooperativeMode && !assembler.Repeating) canAdd = true;
                }
                else
                {
                    if (autoPowerControl == "Y" && !assembler.IsProducing && assembler.IsQueueEmpty && !assembler.CooperativeMode && !assembler.Repeating) canAdd = true;
                }


                if (canAdd == true)
                {
                    if (assembler.Enabled == false) assembler.Enabled = true;
                    foreach (var item in productionList)
                    {
                        MyDefinitionId myDefinitionId = new MyDefinitionId();
                        myDefinitionId = MyDefinitionId.Parse(item.Key);
                        Echo(item.Key.ToString());
                        assembler.AddQueueItem(myDefinitionId, item.Value);
                    }
                }
            }
        }

        //###############     FacilitiesControl     ###############

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



        public void Main(string argument, UpdateType updateSource)
        {
            Echo($"{DateTime.Now}");
            Echo("Program is running.");

            if (counter_ProgramRefresh != 1 || counter_ProgramRefresh != 6 || counter_ProgramRefresh != 11 || counter_ProgramRefresh != 16)
            {
                if (counter_Logo++ >= 360) counter_Logo = 0;

                ProgrammableBlockScreen();

                OverallDisplay();
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
                    break;
            }


            FacilitiesControl(argument);

        }
    }
}



