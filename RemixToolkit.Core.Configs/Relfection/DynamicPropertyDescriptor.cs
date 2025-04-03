using System.ComponentModel;

namespace RemixToolkit.Core.Configs.Relfection;

public class DynamicPropertyDescriptor : PropertyDescriptor
{
    private readonly Type _type;
    private readonly object? _initialValue;
    private object? _value;

    public DynamicPropertyDescriptor(string name, Type type, Attribute[] attrs, object? initialValue = null)
        : base(name, attrs)
    {
        _type = type;
        _value = initialValue;
        _initialValue = initialValue;
    }

    public override Type ComponentType => typeof(DynamicConfig);

    public override bool IsReadOnly { get; } = false;

    public override Type PropertyType => _type!;

    public override bool CanResetValue(object component) => true;

    public override object? GetValue(object? component) => _value;

    public override void ResetValue(object component) => SetValue(component, _initialValue);

    public override void SetValue(object? component, object? value)
    {
        _value = Convert.ChangeType(value, PropertyType);
        this.OnValueChanged(component, EventArgs.Empty);
    }

    public override bool ShouldSerializeValue(object component) => true;
}