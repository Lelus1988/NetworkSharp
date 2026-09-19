using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using NetworkSharp.Common;

namespace NetworkSharp.ViewModels;

public enum TrafficRange
{
    Live,
    Day,
    Week,
    Month
}

public partial class NetworkMonitorViewModel
{
    private const int Points = 60;
    private readonly DispatcherTimer _liveTimer = new() { Interval = TimeSpan.FromSeconds(1) };

    public ChartValues<ObservableValue> DownloadValues { get; } = CreateValues();
    public ChartValues<ObservableValue> UploadValues { get; } = CreateValues();
    public SeriesCollection DailyConsumptionSeries { get; } = new();
    public string[] Labels { get; private set; } = new string[Points];
    public string[] DayLabels { get; private set; } = Array.Empty<string>();
    public Func<double, string> Formatter { get; } = value => $"{value:F0}";
    public ICommand SetRangeCommand { get; private set; } = null!;

    private static ChartValues<ObservableValue> CreateValues() =>
        new(Enumerable.Range(0, Points).Select(_ => new ObservableValue(0)));

    private void InitChart()
    {
        SetRangeCommand = new RelayCommand(parameter =>
        {
            if (parameter is string value && Enum.TryParse<TrafficRange>(value, out var range))
                _ = SetRangeAsync(range);
        });

        DailyConsumptionSeries.Add(new ColumnSeries
        {
            Title = "Verbrauch",
            Values = new ChartValues<double>(),
            Fill = System.Windows.Media.Brushes.Transparent
        });

        _ = SetRangeAsync(TrafficRange.Week);
    }

    private async Task SetRangeAsync(TrafficRange range)
    {
        _liveTimer.Stop();

        if (range == TrafficRange.Live)
        {
            StartLive();
            return;
        }

        var timeSpan = range switch
        {
            TrafficRange.Day => TimeSpan.FromHours(24),
            TrafficRange.Week => TimeSpan.FromDays(7),
            TrafficRange.Month => TimeSpan.FromDays(30),
            _ => TimeSpan.FromDays(7)
        };

        var history = await _networkMonitorService.GetDataRateHistoryAsync(timeSpan);
        var download = history.Select(point => point.DownloadSpeed).ToArray();
        var upload = history.Select(point => point.UploadSpeed).ToArray();

        Apply(DownloadValues, Resample(download));
        Apply(UploadValues, Resample(upload));

        var from = history.Length > 0 ? history[0].Timestamp : DateTime.Now - timeSpan;
        var to = history.Length > 0 ? history[^1].Timestamp : DateTime.Now;
        var format = range == TrafficRange.Day ? "HH:mm" : range == TrafficRange.Week ? "ddd d.M." : "d.M.";
        Labels = Enumerable.Range(0, Points)
            .Select(index => (from + (to - from) * index / (Points - 1)).ToString(format))
            .ToArray();
        OnPropertyChanged(nameof(Labels));

        await UpdateDailyConsumptionAsync(range);
    }

    private void StartLive()
    {
        Labels = Enumerable.Range(0, Points)
            .Select(index => index == Points - 1 ? "jetzt" : $"-{Points - 1 - index} s")
            .ToArray();
        OnPropertyChanged(nameof(Labels));

        _liveTimer.Tick -= OnLiveTick;
        _liveTimer.Tick += OnLiveTick;
        _liveTimer.Start();
    }

    private void OnLiveTick(object? sender, EventArgs e)
    {
        for (var index = 0; index < Points - 1; index++)
        {
            DownloadValues[index].Value = DownloadValues[index + 1].Value;
            UploadValues[index].Value = UploadValues[index + 1].Value;
        }

        DownloadValues[Points - 1].Value = CurrentDownloadSpeed;
        UploadValues[Points - 1].Value = CurrentUploadSpeed;
    }

    private async Task UpdateDailyConsumptionAsync(TrafficRange range)
    {
        var days = range == TrafficRange.Month ? 30 : 7;
        var dailyConsumption = await _networkMonitorService.GetDailyConsumptionAsync(days);
        var values = DailyConsumptionSeries[0].Values as ChartValues<double>;
        values?.Clear();

        var labels = new List<string>();
        foreach (var day in dailyConsumption)
        {
            values?.Add(day.ConsumptionGB);
            labels.Add(day.Date.ToString("ddd d.M."));
        }

        DayLabels = labels.ToArray();
        OnPropertyChanged(nameof(DayLabels));
    }

    private static void Apply(ChartValues<ObservableValue> target, double[] values)
    {
        for (var index = 0; index < Points; index++)
            target[index].Value = values[index];
    }

    private static double[] Resample(double[] source)
    {
        var output = new double[Points];
        if (source.Length == 0)
            return output;

        for (var index = 0; index < Points; index++)
        {
            var start = index * source.Length / Points;
            var end = Math.Max(start + 1, (index + 1) * source.Length / Points);
            output[index] = source[start..Math.Min(end, source.Length)].DefaultIfEmpty().Average();
        }

        return output;
    }

    public Task UpdateChartDataAsync(string timeRange)
    {
        var range = timeRange switch
        {
            "Live" => TrafficRange.Live,
            "24 Std" => TrafficRange.Day,
            "30 Tage" => TrafficRange.Month,
            _ => TrafficRange.Week
        };

        return SetRangeAsync(range);
    }

    public void StopChartTimer()
    {
        _liveTimer.Stop();
        _liveTimer.Tick -= OnLiveTick;
    }
}