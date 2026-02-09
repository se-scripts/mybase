# mybase

我的基地脚本，基于James的脚本魔改。

## 魔改的功能

- 去掉了一些基地脚本无用的功能，比如广播GPS等
- 添加了电源电量进度展示
- 添加了精炼厂的矿石精炼定义列表，可以设置精炼矿石的数量，基地有矿石，列表才有矿石配置，脚本自动生成
- 提供了拆卸设备脚本报错的解决方案

## 演示视频

### 哔哩哔哩
<iframe width="560" height="315" src="//player.bilibili.com/player.html?isOutside=true&aid=116043037023608&bvid=BV1dfF1zzE6R&cid=35947348277&p=1&autoplay=0" scrolling="no" border="0" frameborder="no" framespacing="0" allowfullscreen="true"></iframe>

### YouTube
<iframe width="560" height="315" src="https://www.youtube-nocookie.com/embed/tnlS_5IOq98?si=3HrMwiIWNf7IQ4Xf" title="YouTube video player" frameborder="0" allow="accelerometer; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>




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

steam创意工坊搜mybase

进这个网址 [Program.cs](Program.cs)

复制代码到SE的编程块里

区域是上下两行大括号

开始行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/c9da6269-6c71-4e49-b25e-9e928ebe86c4)

结束行
![image](https://github.com/se-scripts/inventory-graphic/assets/46225881/6740f7e2-f7e6-4f36-ab58-08f4d856180e)

PS: 看行号没用，脚本更新行号会变，还有如提示脚本字符超了，建议临时新建个txt，复制后Cltr+A全选，然后按Shift+Tab三次，移除每行前面的空格。

# 关于拆卸设备时的脚本报错

你需要安一个`事件控制器`，事件选择`方块已添加/已移除`, 动作设置这块，1号位编程块运行参数`ReloadBlocksFromGridTerminalSystem1`, 2号位编程块运行参数`ReloadBlocksFromGridTerminalSystem2`，需要注意，这里的编程块是你运行当前脚本的编程块。

配置好后开启事件控制器方块，你拆卸设备会自动更新脚本用到的方块集合，不会报错了。

# [OverallConfig]

- DisplayAssemblerName: 配置成一个复制的一个装配机的名称，会在总揽面板里展示装配机的生产进度。

# [BasicConfig]

基础配置，配置脚本对象范围等

下方配置的默认配置均为``true``

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

如果上述几个配置，其中有一个配置成了`false`，感知到连接网格方块的同时，当连接网格断开时，由于不存在对应的网格方块，脚本会报错，解决方案如下：

安一个`定时器`，定时器设置成1秒，设置动作时，1号位编程块运行参数`ReloadBlocksFromGridTerminalSystem1`, 2号位编程块运行参数`ReloadBlocksFromGridTerminalSystem2`，需要注意，这里的编程块是你运行当前脚本的编程块，

再安一个`事件控制器`，事件选择`连接器已连接`，将可用方块里的所有当前网格的连接器选中，添加方块到已选方块中, 动作设置这块，1号位设置成定时器开始，2号位设置成定时器马上触发，

经过上述步骤后，当连接器连接或者断开后，会自动重载脚本引用对象。

- RefreshRate: 编程脚本的刷新频率，F越多越快越频繁，网格越复杂，建议把频率调慢，这样更不容易超限。
	- 配置成`F`, 代表更新频率为每100个游戏Tick
	- 默认配置成`FF`, 代表更新频率为每10个游戏Tick
	- 配置成`FFF`, 代表更新频率为每1个游戏Tick


# 精炼自动管理配置

编程块重置后，你需要先手动将精炼厂的入料口的东西全移动到箱子里，清空入料口，给脚本移动矿腾出空间。

这部分配置写在每个精炼厂的自定义数据里。

## [InputItemList]
这部分会根据箱子里有多少种原矿，动态生成多少行配置，只需要改数字即可，其它的不要动。

数字配置成0代表脚本不拉取该原矿，数字配置成正数，代表脚本每次拉取多少原矿，建议不要过大，导致单个原矿直接堵住了入料口。


## [RefineriesAutoManager]

### NotAddWhenInputItemHasValue
默认为`true`，代表如果精炼厂入料口有原矿，则不拉取该原矿；
这个如果配置成false，那么脚本每次检测的时候，都会将你的精炼厂入料口数量添加到配置的数量，
意味着只有你箱子里还有矿，那么永远都精炼入料口第一个矿，直到第一个矿全部精炼完毕。

### [JoinHandsInputItemRelation]
这个是联合精炼的功能，如果这个单行里面几种原料，只要有一种原料的数量没有大于100，那么所有的原料无论上面的`InputItemList`里配置的数量是多少都不会拉取，
彻底解决需要多个原料才能精炼的原矿，卡在入料口第一格，导致其它的原矿无法精炼的问题。

例子:
```
[JoinHandsInputItemRelation]
Length=1
1=MyObjectBuilder_Ore/铝矿:MyObjectBuilder_Ore/氦矿
```

# [CargoAutoManager]
这个是物品箱子自动分类的功能；

你需要先让脚本跑10秒，脚本会往编程块写这个配置

## enable
默认为`false`，代表不启用这个功能

配置成`true`，启用该功能

## IsCargoAutoManagerSameConstructAs
默认为`true`，代表统计(管理范围)仅仅只有是当前网格，通过诸如连接器连接的其它网格不进行管理

配置成`false`，则代表统计(管理范围)是当前网格和与当前网格通过诸如连接器连接的所有其它网格。


## 箱子命名

给对应箱子名称加上下方标签，需要带英文中括号：

- 存放矿物的货箱标签: `[矿物]`
- 存放锭的货箱标签: `[矿锭]`
- 存放组件的货箱标签: `[零件]`
- 存放工具的货箱标签: `[工具]`
- 存放弹药的货箱标签: `[弹药]`
- 存放种子的货箱标签: `[种子]`
- 存放消费品的货箱标签: `[消费品]`
- 不计入统计(管理范围)的货箱标签: `[禁止]`



# [AssemblerInputManager]

这个是脚本给装配机添加原料的功能，配置在编程块自定义数据里，默认关闭；

## enable
是否启用该功能，默认为`false`关闭，
启用则需配置成`true` 

## AssemblerCustomNameKeyword
装配机名称包含的关键字，默认为`压缩`，代表该功能只管理名称包含`压缩`的装配机

## 单个装配机的自定义数据配置

示例：
```
[InputItemList]
Length=1
1=MyObjectBuilder_Ingot/Nickel:40000

```

### InputItemList

`Length`: 配置了几行则脚本读取几行

中间是物品的类型ID，参考翻译列表，右边是设定的最大数量，不建议过大，设置超过装配机入料口的最大空间，则不会进行移动。



# LCD物品变化统计

物品变化统计面板的名字是：LCD_Statistics
	
[Stats] => StatsTimeInterval: 统计间隔，需要注意的是，编程块的自定义数据修改后，需要重置代码

例子：

```
[Stats]
StatsTimeInterval=1
Length=21
1=MyObjectBuilder_Ingot/IronX10
2=MyObjectBuilder_Ingot/SiliconX10
3=MyObjectBuilder_Ingot/NickelX10
4=MyObjectBuilder_Ingot/GoldX10
5=MyObjectBuilder_Component/Neutronium
6=MyObjectBuilder_Ingot/SilverX10
7=MyObjectBuilder_Ingot/CobaltX10
8=MyObjectBuilder_Ingot/Stone
9=MyObjectBuilder_Ingot/Iron
10=MyObjectBuilder_Ingot/Nickel
11=MyObjectBuilder_Ingot/Cobalt
12=MyObjectBuilder_Ingot/Magnesium
13=MyObjectBuilder_Ingot/Silicon
14=MyObjectBuilder_Ingot/Silver
15=MyObjectBuilder_Ingot/Gold
16=MyObjectBuilder_Ingot/Platinum
17=MyObjectBuilder_Ingot/Uranium
18=MyObjectBuilder_Ingot/Naquadah
19=MyObjectBuilder_Ingot/Trinium
20=MyObjectBuilder_Ingot/Neutronium
21=MyObjectBuilder_Ingot/DeuteriumContainer
```

如果您需要监控的物品类型数目大于20条，那么一个屏幕装不下，您需要配置多个面板进行分屏幕：

LCD的命名规则是 `LCD_Statistics_Display:{X}`，这里的X从1开始，代表第几个统计屏幕，

比如将一个LCD命名为`LCD_Statistics_Display:1`，代表第一个统计的LCD，同理第二个就是`LCD_Statistics_Display:2`，第三个就是`LCD_Statistics_Display:3`

关于面板的表情含义：绿色开心代表物品在这个时间间隔内数目增加， 白色无表情代表物品在这个时间间隔内数目没变化，黄色闷闷不乐代表物品在这个时间间隔内数目在减少。


# 生产设备蓝图图标映射配置 [ModBlueprintSubtypeIdResultMap]

当前配置处于，编程块的自定义配置，修改后需重置代码

- `enable`: 默认为`false`，代表不启用；如果配置成`true`则代表启用映射。
- `Length`: 代表脚本读取几行的映射配置
- `{index}={key}:{value}`: 
	- `index`代表索引，从1开始，到`Lengh`的值结束
	- `key`是生产设备蓝图的`SubtypeId`，对着生产设备的面板安F打开，会显示每台设备的蓝图，例如`MyObjectBuilder_BlueprintDefinition/ZHBYX3`,这里`/`后的`ZHBYX3`就是`SubtypeId`
	- `value`: 你需要进你网格的`生产`面板，找到对应的设备，查看对应的蓝图的图标最像什么物品，然后对着物品图形化面板按F打开，复制物品的ID，例如`MyObjectBuilder_Ingot/IronX10`，这里`/`后的`IronX10`就是需要的`value`值


配置好启用和列表后，重置编程块代码，稍等片刻，生产设备图形化面板前面的图标就显示出来了。


举个在璇玑星界服务器配置的例子：
```
[ModBlueprintSubtypeIdResultMap]
enable=true
Length=41
1=ZHBY1:IronX10
2=ZHBY2:NickelX10
3=ZHBY3:SiliconX10
4=ZHBY4:CobaltX10
5=ZHBY5:MagnesiumX10
6=ZHBY6:SilverX10
7=ZHBY7:GoldX10
8=ZHBY8:PlatinumX10
9=ZHBY9:UraniumX10
10=ZHBYX1:IronX10
11=ZHBYX2:NickelX10
12=ZHBYX3:SiliconX10
13=ZHBYX4:CobaltX10
14=ZHBYX5:MagnesiumX10
15=ZHBYX6:SilverX10
16=ZHBYX7:GoldX10
17=ZHBYX8:PlatinumX10
18=ZHBYX9:UraniumX10
19=ZHBYY1:IronX100
20=ZHBYY2:NickelX100
21=ZHBYY3:SiliconX100
22=ZHBYY4:CobaltX100
23=ZHBYY5:MagnesiumX100
24=ZHBYY6:SilverX100
25=ZHBYY7:GoldX100
26=ZHBYY8:PlatinumX100
27=ZHBYY9:UraniumX100
28=ZHBYYX1:IronX100
29=ZHBYYX2:NickelX100
30=ZHBYYX3:SiliconX100
31=ZHBYYX4:CobaltX100
32=ZHBYYX5:MagnesiumX100
33=ZHBYYX6:SilverX100
34=ZHBYYX7:GoldX100
35=ZHBYYX8:PlatinumX100
36=ZHBYYX9:UraniumX100
37=contre11:ImpossibilityCore2
38=Fkuangwu6:Silver
39=Fkuangwu7:Gold
40=Fkuangwu8:Platinum
41=NeutroniumX2:Neutronium
```
