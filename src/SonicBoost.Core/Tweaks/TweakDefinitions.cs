using Microsoft.Win32;
using SonicBoost.Core.Tweaks.Models;

namespace SonicBoost.Core.Tweaks;

public static class TweakDefinitions
{
    public static List<TweakItem> All() =>
    [
        new()
        {
            Id = "disable_game_bar",
            Name = "Отключить игровую панель",
            Description = "Отключает оверлей Xbox Game Bar, потребляющий ресурсы в играх",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR",
            RegistryKey = "AppCaptureEnabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_game_dvr",
            Name = "Отключить Game DVR",
            Description = "Отключает фоновую запись и функции Game DVR",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR",
            RegistryKey = "AllowGameDVR",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_fullscreen_optimizations",
            Name = "Отключить оптимизацию полноэкранного режима",
            Description = "Глобально отключает оптимизации, которые могут вызывать задержку ввода",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\System\GameConfigStore",
            RegistryKey = "GameDVR_FSEBehaviorMode",
            EnabledValue = 2,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "hardware_gpu_scheduling",
            Name = "Аппаратное планирование GPU",
            Description = "Включает HAGS для снижения задержки (требуется перезагрузка)",
            Category = "Игры",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers",
            RegistryKey = "HwSchMode",
            EnabledValue = 2,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "game_mode",
            Name = "Включить игровой режим",
            Description = "Включает игровой режим Windows для лучшего распределения ресурсов",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\GameBar",
            RegistryKey = "AutoGameModeEnabled",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_mouse_acceleration",
            Name = "Отключить ускорение мыши",
            Description = "Отключает повышенную точность указателя для предсказуемого движения мыши",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Control Panel\Mouse",
            RegistryKey = "MouseSpeed",
            EnabledValue = "0",
            DisabledValue = "1",
            ValueKind = RegistryValueKind.String
        },
        new()
        {
            Id = "disable_visual_effects",
            Name = "Отключить визуальные эффекты",
            Description = "Включает режим наилучшей производительности, освобождая ресурсы GPU/CPU",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
            RegistryKey = "VisualFXSetting",
            EnabledValue = 2,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_transparency",
            Name = "Отключить прозрачность",
            Description = "Отключает прозрачность окон для ускорения отрисовки",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
            RegistryKey = "EnableTransparency",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_animations",
            Name = "Отключить анимации окон",
            Description = "Отключает анимации сворачивания и разворачивания окон",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\Control Panel\Desktop\WindowMetrics",
            RegistryKey = "MinAnimate",
            EnabledValue = "0",
            DisabledValue = "1",
            ValueKind = RegistryValueKind.String
        },
        new()
        {
            Id = "disable_cortana",
            Name = "Отключить Cortana",
            Description = "Отключает Cortana для экономии памяти и CPU",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
            RegistryKey = "AllowCortana",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_web_search",
            Name = "Отключить веб-поиск в меню «Пуск»",
            Description = "Отключает результаты Bing в поиске меню «Пуск»",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Policies\Microsoft\Windows\Explorer",
            RegistryKey = "DisableSearchBoxSuggestions",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_widgets",
            Name = "Отключить виджеты",
            Description = "Отключает панель виджетов (Win11) для экономии ресурсов",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Policies\Microsoft\Dsh",
            RegistryKey = "AllowNewsAndInterests",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "gpu_priority",
            Name = "Высокий приоритет GPU для игр",
            Description = "Устанавливает высокий приоритет GPU для активных приложений",
            Category = "Игры",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games",
            RegistryKey = "GPU Priority",
            EnabledValue = 8,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "cpu_priority_games",
            Name = "Высокий приоритет CPU для игр",
            Description = "Устанавливает высокий приоритет планировщика для игровых процессов",
            Category = "Игры",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games",
            RegistryKey = "Priority",
            EnabledValue = 6,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_power_throttling",
            Name = "Отключить троттлинг питания",
            Description = "Запрещает Windows снижать частоту CPU для энергосбережения",
            Category = "Производительность",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling",
            RegistryKey = "PowerThrottlingOff",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "network_throttling",
            Name = "Отключить сетевой троттлинг",
            Description = "Убирает задержку 10 мс для игр и мультимедиа",
            Category = "Сеть",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
            RegistryKey = "NetworkThrottlingIndex",
            EnabledValue = unchecked((int)0xFFFFFFFF),
            DisabledValue = 10,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "high_timer_resolution",
            Name = "Высокое разрешение таймера",
            Description = "Включает высокое разрешение таймера для плавного кадрообразования",
            Category = "Игры",
            Risk = TweakRisk.Advanced,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\kernel",
            RegistryKey = "GlobalTimerResolutionRequests",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_prefetch",
            Name = "Отключить Prefetch",
            Description = "Отключает Prefetch на SSD для уменьшения записи на диск",
            Category = "Производительность",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters",
            RegistryKey = "EnablePrefetcher",
            EnabledValue = 0,
            DisabledValue = 3,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_superfetch",
            Name = "Отключить SysMain / Superfetch",
            Description = "Предзагрузка приложений в память — бесполезна на SSD и тратит ресурсы",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Services\SysMain",
            RegistryKey = "Start",
            EnabledValue = 4,
            DisabledValue = 2,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_fast_startup",
            Name = "Отключить быстрый запуск",
            Description = "Гибридная загрузка может вызывать проблемы с драйверами и обновлениями",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Power",
            RegistryKey = "HiberbootEnabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_background_apps",
            Name = "Отключить фоновые приложения",
            Description = "Запрещает UWP-приложениям работать в фоне и потреблять ресурсы",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications",
            RegistryKey = "GlobalUserDisabled",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_startup_delay",
            Name = "Убрать задержку автозагрузки",
            Description = "Убирает искусственную задержку запуска программ при старте Windows",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Serialize",
            RegistryKey = "StartupDelayInMSec",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_storage_sense",
            Name = "Отключить контроль памяти",
            Description = "Отключает автоматическую очистку диска, которая может удалять нужные файлы",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\StorageSense\Parameters\StoragePolicy",
            RegistryKey = "01",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_lock_screen_tips",
            Name = "Отключить рекламу на экране блокировки",
            Description = "Убирает «советы и подсказки» и рекламный контент с экрана блокировки",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
            RegistryKey = "RotatingLockScreenOverlayEnabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_suggest_apps",
            Name = "Отключить предлагаемые приложения",
            Description = "Запрещает Windows устанавливать рекламные приложения автоматически",
            Category = "Производительность",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
            RegistryKey = "SilentInstalledAppsEnabled",
            EnabledValue = 0,
            DisabledValue = 1,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "large_system_cache",
            Name = "Увеличить системный кэш",
            Description = "Выделяет больше ОЗУ под кэш файловой системы для ускорения загрузки",
            Category = "Производительность",
            Risk = TweakRisk.Moderate,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
            RegistryKey = "LargeSystemCache",
            EnabledValue = 1,
            DisabledValue = 0,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "disable_xbox_game_monitoring",
            Name = "Отключить мониторинг Xbox Game Monitoring",
            Description = "Отключает службу мониторинга Xbox, потребляющую ресурсы",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SYSTEM\CurrentControlSet\Services\xbgm",
            RegistryKey = "Start",
            EnabledValue = 4,
            DisabledValue = 3,
            ValueKind = RegistryValueKind.DWord
        },
        new()
        {
            Id = "multimedia_scheduling",
            Name = "Приоритет мультимедийных задач",
            Description = "Выделяет 100% приоритета планировщика для мультимедиа и игр",
            Category = "Игры",
            Risk = TweakRisk.Safe,
            RegistryPath = @"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
            RegistryKey = "SystemResponsiveness",
            EnabledValue = 0,
            DisabledValue = 20,
            ValueKind = RegistryValueKind.DWord
        },
    ];
}
