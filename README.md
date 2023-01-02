# nng

[![Nuget](https://img.shields.io/nuget/v/nng)](https://www.nuget.org/packages/nng)
[![License badge](https://img.shields.io/badge/license-EUPL-blue.svg)](LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/MrAlonas/nng)](https://github.com/MrAlonas/nng/issues)
[![Build and release](https://github.com/MrAlonas/nng/actions/workflows/build.yml/badge.svg)](https://github.com/MrAlonas/nng/actions/workflows/build.yml)

Ядро, используемое во всех проектах nng.

## data.json

Во всех проектах nng используется единый файл, в котором содержится информация как о группах, так и о заблокированных пользователях:

```json
{
  "$schema": "https://raw.githubusercontent.com/MrAlonas/nng/master/schema.json",
  "lst": [
    10000,
    20000
  ],
  "bnnd": [
    {
      "id": 1,
      "name": "Павел Дуров",
      "priority": 4,
      "bot": false,
      "warned": 1,
      "compliant": [
        23000
      ],
      "deleted": false
    },
    {
      "id": 2,
      "name": "Александра Владимирова",
      "priority": 1,
      "bot": true,
      "warned": 0,
      "deleted": true
    }
  ],
  "thx": [
    {
      "id": 3,
      "name": "Вячеслав"
    }
  ]
}
```

При составлении файла опирайтесь на [схему](https://github.com/MrAlonas/nng/blob/master/schema.json).

## Спасибо

[![1Password](https://user-images.githubusercontent.com/60031799/197296937-66512000-7f0a-4f15-bacf-bfec0c242906.png)](https://1password.com)

[![JebBrains](https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.svg)](https://jb.gg/OpenSourceSupport)
