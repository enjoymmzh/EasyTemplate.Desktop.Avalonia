# EasyTemplate.Desktop.Avalonia 简单的桌面应用整合模板

#### 介绍

简易的桌面应用集成框架
该仓库主要目的：
1.  一个简单的基于avalonia + SukiUI桌面应用如何开发
2.  将常用功能整合在一起，例如datagrid，cef等
3.  大多数功能尽可能直接使用，不进行多级封装，便于理解

#### 软件架构

.Net8 + Avalonia + SukiUI

#### 包含示例

1.  Avalonia.Controls.DataGrid的使用，附加分页功能
2.  CefGlue.Avalonia的使用，目前暂不包含页面内容解析和代理使用
3.  图表LiveCharsCore的基本使用
4.  图标库Material.Icons.Avalonia的引入和使用
5.  Dialog和Toast的基本用法
6.  多语言环境的搭建，多语言使用csv文件记录，在EasyTemplate.Ava.Tool.Resources中，加载类在EasyTemplate.Ava.Tool.Util的Localization.cs
7.  更新功能，在EasyTemplate.Ava.Tool.Util的UpdateTask.cs，使用了第三方类库Downloader，调用部分在EasyTemplate.Ava的MainWindowViewModel.cs的CheckVersion()方法中

#### 项目说明

1.  EasyTemplate.Ava.Desktop：项目主入口，将该项目设置为启动项目
2.  EasyTemplate.Ava：桌面组件主要项目，组件添加在EasyTemplate.Ava.Features中，新增组件后，需要手动修改组件和ViewModel命名空间为 namespace EasyTemplate.Ava.Features;
3.  EasyTemplate.Ava.Tool：在这里添加实体。配置文件 Configuration + 实体 Entity + 工具类 Util
4.  项目根目录（与.sln同级目录）的文件夹ThirdPartyDemo，包含SukiUI.Demo和Material.Icons.Avalonia的编译版本，无需再去github找源代码

#### 组件参考

Avalonia官方文档：https://docs.avaloniaui.net/zh-Hans/docs/welcome
SukiUI官方文档：https://kikipoulet.github.io/SukiUI/documentation/controls/navigation/stackpage.html
SukiUI git：https://github.com/kikipoulet/SukiUI