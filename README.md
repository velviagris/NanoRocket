﻿# NanoRocket

[![NuGet](https://img.shields.io/nuget/v/NanoRocket.svg)](https://nuget.org/packages/NanoRocket) [![Nuget Downloads](https://img.shields.io/nuget/dt/NanoRocket)](https://www.nuget.org/packages/NanoRocket) [![License](https://img.shields.io/github/license/velviagris/NanoRocket)](https://github.com/velviagris/NanoRocket)

## About

NanoRocket is an extension library for RocketMQ.Client.

## Building

| Branch |                                                                                      Building Status                                                                                      |
|:------:|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
| master | [![build](https://github.com/velviagris/NanoRocket/actions/workflows/build.yml/badge.svg?branch=master&event=push)](https://github.com/velviagris/NanoRocket/actions/workflows/build.yml) |
|  dev   |  [![build](https://github.com/velviagris/NanoRocket/actions/workflows/build.yml/badge.svg?branch=dev&event=push)](https://github.com/velviagris/NanoRocket/actions/workflows/build.yml)   |

## Features

- Read configurations in `appsettings.json`.
- Dependency injection available.
- Multiple connections, producers, and consumers can be created.

## Installation

You can get NanoRocket by grabbing the latest [NuGet](https://www.nuget.org/packages/NanoRocket) package.

See [Wiki](https://github.com/velviagris/NanoRocket/wiki/Installation) for more details.

## Version

| NanoRocket | RocketMQ.Client | .NET |
|:----------:|:---------------:|:----:|
|   1.0.0    |      5.1.0      | 8.0  |

For more, please visit the [Examples](https://github.com/velviagris/NanoRocket/tree/master/Example).

## Contributing

1. Fork this repository.
2. Create a new branch in you current repos from the **dev** branch.
3. Push commits and create a Pull Request (PR) to NanoRocket.

## Todo

- [x] Basic Consume & Publish
- [x] DependencyInjection

## Thanks

- Visual Studio 2022
- [RocketMQ.Client](https://github.com/apache/rocketmq-clients/tree/master/csharp)

## License

NanoRocket is licensed under the [MIT](https://github.com/velviagris/NanoRocket/blob/master/LICENSE.txt) license.
