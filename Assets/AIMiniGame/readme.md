# Unity 游戏框架文档

本文档将介绍如何使用和扩展这个 Unity 游戏框架，包括各个模块的设计思路、使用方法和示例代码。

## 目录

1. [简介](#简介)
2. [单例模式](#单例模式)
4. [事件系统](#事件系统)
3. [资源管理模块](#资源管理模块)
6. [UI管理系统](#示例代码)
5. [配置管理模块](#配置管理模块)
6. [输入系统](#示例代码)

## 简介

此框架旨在提供一个模块化的架构，以便快速开发和扩展 Unity 游戏。它包括单例模式、资源管理、事件系统和配置管理模块。每个模块都设计为易于理解和使用，且能无缝集成到你的 Unity 项目中。

## 单例模式

单例模式用于确保一个类只有一个实例，并提供全局访问点。该框架提供了一个通用的单例基类。

### 设计思路

通过继承 `Singleton<T>`，可以快速创建一个单例类。

### 使用方法

1. 创建一个继承自 `Singleton<T>` 的类。
2. 在该类中定义所需的功能。

### 示例代码

```csharp
public class GameManager : Singleton<GameManager> {
    public void Initialize() {
        // 初始化逻辑
    }
}
```

## 资源管理模块

资源管理模块用于动态加载和卸载资源，支持异步操作。该模块使用 Unity 的 AssetBundle 来管理资源。

### 设计思路

资源管理模块设计为异步加载和卸载资源，以提高游戏的性能和灵活性。

### 使用方法

1. 使用 `ResourceManager.Instance.LoadResourceAsync` 加载资源。
2. 使用 `ResourceManager.Instance.UnloadResource` 卸载资源。

### 示例代码

```csharp
// 加载资源
ResourceManager.Instance.LoadResourceAsync<GameObject>("path/to/resource", (obj) => {
    // 资源加载完成后的逻辑
});

// 卸载资源
ResourceManager.Instance.UnloadResource("path/to/resource");
```

## 事件系统

事件系统用于在不同模块之间传递消息，支持传递多个参数和不同类型的参数。

### 设计思路

使用委托和泛型来实现一个灵活且高性能的事件系统，避免使用 `object` 类型来减少 GC 开销。

### 使用方法

1. 使用 `EventManager.Instance.Register` 注册事件。
2. 使用 `EventManager.Instance.Unregister` 注销事件。
3. 使用 `EventManager.Instance.TriggerEvent` 触发事件。

### 示例代码

```csharp
// 注册事件
EventManager.Instance.Register<int, string>("OnPlayerScored", OnPlayerScored);

// 事件处理方法
private void OnPlayerScored(int score, string playerName) {
    // 处理逻辑
}

// 触发事件
EventManager.Instance.TriggerEvent("OnPlayerScored", 100, "Player1");

// 注销事件
EventManager.Instance.Unregister<int, string>("OnPlayerScored", OnPlayerScored);
```
## 配置管理模块

配置管理模块用于加载和解析配置文件，支持 CSV 格式并提供缓存机制。

### 设计思路

通过读取 CSV 文件生成配置类，使用静态方法提供数据访问，支持多种数据类型和数组。配置文件的第一行是字段名，第二行是字段类型，第三行是字段描述，从第四行开始是数据内容。

### 使用方法

1. 创建配置文件并放置在 Resources 文件夹中。
2. 右键 CSV 文件生成配置类。
3. 使用配置类的静态方法访问配置数据。

### 示例代码

```csharp
// 获取配置数据
TestFileConfig config = TestFileConfig.Get("some_id");

// 获取所有配置键
List<string> keys = TestFileConfig.GetKeys();
```


