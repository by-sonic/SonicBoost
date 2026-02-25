<div align="center">

# SonicBoost

### Оптимизация Windows для игр в один клик

[![Build](https://github.com/by-sonic/SonicBoost/actions/workflows/build-release.yml/badge.svg)](https://github.com/by-sonic/SonicBoost/actions)
[![Release](https://img.shields.io/github/v/release/by-sonic/SonicBoost?color=1e90ff&label=Скачать)](https://github.com/by-sonic/SonicBoost/releases/latest)
[![License](https://img.shields.io/github/license/by-sonic/SonicBoost?color=00d4aa)](LICENSE)
[![Stars](https://img.shields.io/github/stars/by-sonic/SonicBoost?style=social)](https://github.com/by-sonic/SonicBoost)

**SonicBoost** — бесплатная утилита с открытым исходным кодом для тонкой настройки Windows 10/11 под игры.  
28+ твиков реестра, управление службами, очистка телеметрии, оптимизация сети — всё с красивым UI и обратной связью.

[Скачать .exe](https://github.com/by-sonic/SonicBoost/releases/latest) · [Статья на Хабре](#) · [SonicVPN](https://t.me/bysonicvpn_bot)

</div>

---

## Возможности

| Модуль | Описание |
|--------|----------|
| **Игровые твики** | 28 оптимизаций реестра: Game Bar, Game DVR, HAGS, приоритеты GPU/CPU, таймеры, фоновые приложения |
| **Управление службами** | Отключение 18 ненужных служб (телеметрия, Xbox, факс, геолокация) с цветовыми статусами |
| **Конфиденциальность** | Блокировка телеметрии Microsoft, рекламного ID, Copilot, Recall, журнала активности |
| **Очистка системы** | Удаление 25+ предустановленных приложений (Cortana, Teams, Candy Crush...) + очистка temp |
| **Оптимизация сети** | Отключение Nagle, TCP-оптимизация, настройка DNS (Cloudflare/Google/Quad9) |
| **Драйверы** | Определение оборудования и ссылки на последние драйверы |
| **Электропитание** | Ultimate Performance, отключение гибернации, управление планами питания |

## Скриншоты

> Тёмный интерфейс на базе WPF UI (Fluent Design) с Mica-эффектом

## Быстрый старт

### Скачать готовый EXE

1. Перейди в [Releases](https://github.com/by-sonic/SonicBoost/releases/latest)
2. Скачай `SonicBoost.exe`
3. Запусти **от имени администратора**
4. Выбирай твики и нажимай — статус обновляется моментально

### Собрать из исходников

```bash
git clone https://github.com/by-sonic/SonicBoost.git
cd SonicBoost
dotnet publish src/SonicBoost/SonicBoost.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

Готовый файл: `publish/SonicBoost.exe`

## Технологии

- **.NET 8** — WPF, self-contained single-file
- **WPF UI (Fluent)** — Mica, тёмная тема, Fluent Design
- **CommunityToolkit.Mvvm** — MVVM, source generators
- **Реестр Windows** — прямое чтение/запись через `Microsoft.Win32`

## Безопасность

- Все изменения реестра обратимы (бэкап перед каждым твиком)
- Цветовая маркировка рисков: **Безопасно** / **Умеренно** / **Продвинуто**
- Открытый исходный код — проверяй каждую строку

## SonicVPN

Защита соединения + низкий пинг для игр.  
Подключение через Telegram-бота: [@bysonicvpn_bot](https://t.me/bysonicvpn_bot)

## Лицензия

MIT — используй свободно. Звёздочка на репозитории приветствуется.

---

<div align="center">
  <b>Сделано с любовью к играм</b><br>
  <a href="https://t.me/bysonicvpn_bot">SonicVPN</a> · <a href="https://github.com/by-sonic/SonicBoost">GitHub</a>
</div>
