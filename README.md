# nng

[![License badge](https://img.shields.io/badge/license-EUPL-blue.svg)](LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/MrAlonas/nng)](https://github.com/MrAlonas/nng/issues)
[![Build and release](https://github.com/MrAlonas/nng/actions/workflows/build.yml/badge.svg)](https://github.com/MrAlonas/nng/actions/workflows/build.yml)

Ядро, используемое во всех проектах nng.

## data.json

Во всех проектах nng используется единый файл, в котором содержится информация как о группах, так и о заблокированных пользователях:

```
{
  "$schema": "https://raw.githubusercontent.com/MrAlonas/nng/master/schema.json",
  "lst": [
    10000,
    20000
  ],
  "bnnd": [
    {
      "id": 1,
      "priority": 4,
      "warned": 1,
      "name": "Павел Дуров",
      "compliant": [
        23000
      ]
    },
    {
      "id": 2,
      "priority": 1,
      "warned": 0,
      "name": "Александра Владимирова",
      "deleted": 1
    }
  ]
}
```

При составлении файла опирайтесь на [схему](https://github.com/MrAlonas/nng/blob/master/schema.json).
