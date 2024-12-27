using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace used_car_dealership_app.Views.Clients;

public partial class ClientUpdateView : UserControl
{
    // Digits
    public static readonly HashSet<Key> Digits = new()
    {
        Key.D0, Key.D1, Key.D2, Key.D3, Key.D4,
        Key.D5, Key.D6, Key.D7, Key.D8, Key.D9
    };

    // Letters
    public static readonly HashSet<Key> Letters = new()
    {
        Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J,
        Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T,
        Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z
    };

    // Special characters (including whitespace)
    public static readonly HashSet<Key> SpecialCharacters = new()
    {
        Key.Space, Key.Oem1, Key.Oem2, Key.Oem3, Key.Oem4, Key.Oem5, Key.Oem6, Key.Oem7, Key.Oem8,
        Key.OemComma, Key.OemPeriod, Key.OemPlus, Key.OemMinus
    };
    
    // Special characters for email
    public static readonly HashSet<Key> EmailSpecialCharacters = new()
    {
        Key.OemPeriod, Key.OemMinus, Key.OemPlus, Key.Oem2, // . - + /
        Key.D2, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 // @ ^ & * ( )
    };
    
    public ClientUpdateView()
    {
        InitializeComponent();
    }
    
    private void OnlyLettersKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Letters.Contains(e.Key) && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyLettersAndMinusKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Letters.Contains(e.Key) && e.Key != Key.OemMinus && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyLettersAndMinusAndSpaceKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Letters.Contains(e.Key) && e.Key != Key.OemMinus && e.Key != Key.Space && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyDigitsAndMinusKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Digits.Contains(e.Key) && e.Key != Key.OemMinus && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyDigitsKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Digits.Contains(e.Key) && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyDigitsAndLettersKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Digits.Contains(e.Key) && !Letters.Contains(e.Key) && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyHouseNumberKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Digits.Contains(e.Key) && !Letters.Contains(e.Key) && e.Key!=Key.Oem2 && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
    
    private void OnlyEmailKeyDown(object? sender, KeyEventArgs e)
    {
        if (!Digits.Contains(e.Key) && !Letters.Contains(e.Key) &&  !EmailSpecialCharacters.Contains(e.Key) && e.Key != Key.Back)
        {
            e.Handled = true;
        }
    }
}