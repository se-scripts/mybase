# mybase

我的基地脚本，基于James的脚本魔改。

## 魔改的功能

- 去掉了一些基地脚本无用的功能，比如广播GPS等
- 添加了电源电量进度展示
- 添加了跃迁充电进度展示
- 添加了精炼厂的矿石精炼定义列表，可以设置精炼矿石的数量，基地有矿石，列表才有改矿石

## LCD 设置:

　　1.1 总览屏的命名是 "LCD_Overall_Display".

　　1.2 物料显示屏的命名是 "LCD_Inventory_Display:X", X=1,2,3...

　　例如， LCD_Inventory_Display:1 表示是第一块屏, LCD_Inventory_Display:2 表示是第二块屏……

　　同样的名称可以命名多块屏，这些屏会显示同样的内容。

       1.3 单独显示矿石，显示屏命名”LCD_Ore_Inventory_Display:X", X=1,2,3...

       1.4 单独显示矿锭，显示屏命名"LCD_Ingot_Inventory_Display:X", X=1,2,3...

       1.4 单独显示零件，显示屏命名"LCD_Component_Inventory_Display:X", X=1,2,3...

       1.5 单独显示弹药，显示屏命名"LCD_AmmoMagazine_Inventory_Display:X", X=1,2,3...



        单独显示和混合显示功能不冲突，可同时使用。



       1.6 精炼厂情况，显示屏命名"LCD_Refinery_Inventory_Display:X", X=1,2,3...

       1.7 装配机情况，显示屏命名"LCD_Assembler_Inventory_Display:X", X=1,2,3...


# 如何使用

进这个网址 [Program.cs](Program.cs)

复制代码到SE的编程块里

区域是上下两行大括号

开始行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/c9da6269-6c71-4e49-b25e-9e928ebe86c4)

结束行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/6740f7e2-f7e6-4f36-ab58-08f4d856180e)
