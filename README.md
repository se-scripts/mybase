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

视频演示：<https://www.bilibili.com/video/BV1AQktYcEuT/>

进这个网址 [Program.cs](Program.cs)

复制代码到SE的编程块里

区域是上下两行大括号

开始行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/c9da6269-6c71-4e49-b25e-9e928ebe86c4)

结束行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/6740f7e2-f7e6-4f36-ab58-08f4d856180e)


# 关于拆卸设备时的脚本报错

你需要安一个`事件控制器`，事件选择`方块已添加/已移除`, 动作设置这块，1号位编程块运行参数`ReloadBlocksFromGridTerminalSystem1`, 2号位编程块运行参数`ReloadBlocksFromGridTerminalSystem2`，需要注意，这里的编程块是你运行当前脚本的编程块。

配置好后开启事件控制器方块，你拆卸设备会自动更新脚本用到的方块集合，不会报错了。



# [BasicConfig]

- IsCargoSameConstructAs: 
	- 配置成`true`，只检索当前网格的所有箱子
	- 配置成`false`，检索范围包括所有与当前网格连接的网格的所有箱子
- IsAssemblerSameConstructAs: 
	- 配置成`true`，只检索当前网格的所有装配机
	- 配置成`false`，检索范围包括所有与当前网格连接的网格的所有装配机
- IsRefinerySameConstructAs: 
	- 配置成`true`，只检索当前网格的所有精炼厂
	- 配置成`false`，检索范围包括所有与当前网格连接的网格的所有精炼厂
- IsPowerProducerSameConstructAs: 
	- 配置成`true`，只检索当前网格的所有电源生产着，比如反应堆
	- 配置成`false`，检索范围包括所有与当前网格连接的网格的所有电源生产着，比如反应堆

# [InputItemList]
这部分会根据箱子里有多少种原矿，动态生成多少行配置，只需要改数字即可，其它的不要动。

数字配置成0代表脚本不拉取该原矿，数字配置成正数，代表脚本每次拉取多少原矿，建议不要过大，导致单个原矿直接堵住了入料口。


# 精炼自动管理配置 [RefineriesAutoManager]

编程块重置后，你需要先手动将精炼厂的入料口的东西全移动到箱子里，清空入料口，给脚本移动矿腾出空间。

这部分配置写在每个精炼厂的自定义数据里。

## NotAddWhenInputItemHasValue
默认为`true`，代表如果精炼厂入料口有原矿，则不拉取该原矿；
这个如果配置成false，那么脚本每次检测的时候，都会将你的精炼厂入料口数量添加到配置的数量，
意味着只有你箱子里还有矿，那么永远都精炼入料口第一个矿，直到第一个矿全部精炼完毕。

# [CargoAutoManager]
这个是物品箱子自动分类的功能；

你需要先让脚本跑10秒，脚本会往编程块写这个配置

## enable
默认为`false`，代表不启用这个功能

配置成`true`，启用该功能


## 箱子命名

给对应箱子名称加上下方标签，需要带英文中括号：

- 存放矿物的货箱标签: `[矿物]`
- 存放锭的货箱标签: `[矿锭]`
- 存放组件的货箱标签: `[零件]`
- 存放工具的货箱标签: `[工具]`
- 存放弹药的货箱标签: `[弹药]`
- 不计入统计的货箱标签: `[禁止]`





