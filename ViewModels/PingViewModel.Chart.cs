using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Defaults;
using NetworkSharp.Common;

namespace NetworkSharp.ViewModels;

public sealed record PacketItem(bool Lost);

public partial class PingViewModel
{
    private const int Points = 121;
    private const int PacketWindow = 60;

    private readonly List<long> _rtts = new();
    private int _sent;
    private int _lost;

    public ChartValues<ObservableValue> PingValues { get; } = CreateValues();
    public ChartValues<ObservableValue> AverageValues { get; } = CreateValues();
    public ObservableCollection<PacketItem> Packets { get; } = new();
    public string[] Labels { get; private set; } = Array.Empty<string>();
    public Func<double, string> Formatter { get; } = value => $"{value:F0}";
    public ICommand StartPingCommand { get; private set; } = null!;

    private static ChartValues<ObservableValue> CreateValues() =>
        new(Enumerable.Range(0, Points).Select(_ => new ObservableValue(0)));

    private void InitChart()
    {
        StartPingCommand = new AsyncRelayCommand(StartPingAsync, () => !IsPinging);
        ResetChart(Interval / 1000d);
    }

    public void ResetChart(double intervalSeconds = 1)
    {
        _rtts.Clear();
        _sent = 0;
        _lost = 0;
        Packets.Clear();

        foreach (var value in PingValues)
            value.Value = 0;
        foreach (var value in AverageValues)
            value.Value = 0;

        Labels = Enumerable.Range(0, Points)
            .Select(index =>
            {
                var elapsed = TimeSpan.FromSeconds((Points - 1 - index) * intervalSeconds);
                return index == Points - 1
                    ? "jetzt"
                    : $"-{(int)elapsed.TotalMinutes}:{elapsed.Seconds:00}";
            })
            .ToArray();

        OnPropertyChanged(nameof(Labels));
        LostPackets = 0;
        TotalPackets = 0;
        PacketLoss = 0;
    }

    public void OnPingResult(long? rtt)
    {
        _sent++;
        if (rtt is long milliseconds)
            _rtts.Add(milliseconds);
        else
            _lost++;

        for (var index = 0; index < Points - 1; index++)
        {
            PingValues[index].Value = PingValues[index + 1].Value;
            AverageValues[index].Value = AverageValues[index + 1].Value;
        }

        PingValues[Points - 1].Value = rtt ?? PingValues[Points - 2].Value;

        Packets.Add(new PacketItem(rtt is null));
        if (Packets.Count > PacketWindow)
            Packets.RemoveAt(0);

        if (_rtts.Count > 0)
        {
            CurrentPing = rtt.HasValue ? (int)rtt.Value : CurrentPing;
            AveragePing = (int)Math.Round(_rtts.Average());
            MinimumPing = (int)_rtts.Min();
            MaximumPing = (int)_rtts.Max();

            foreach (var value in AverageValues)
                value.Value = AveragePing;
        }

        LostPackets = _lost;
        TotalPackets = _sent;
        PacketLoss = 100.0 * _lost / _sent;
    }
}