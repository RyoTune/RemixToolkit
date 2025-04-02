using System.ComponentModel;

namespace RemixToolkit.Core.Configs.Relfection;

public class DynamicConfigTypeDescriptor(DynamicConfig instance) : ICustomTypeDescriptor
{
    public AttributeCollection GetAttributes() => TypeDescriptor.GetAttributes(this, true);

    public string? GetClassName() => TypeDescriptor.GetClassName(this, true);

    public string? GetComponentName() => TypeDescriptor.GetComponentName(this, true);

    public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this, true);

    public EventDescriptor? GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);

    public PropertyDescriptor? GetDefaultProperty() => null;

    public object? GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);

    public EventDescriptorCollection GetEvents() => TypeDescriptor.GetEvents(this, true);

    public EventDescriptorCollection GetEvents(Attribute[]? attributes) => TypeDescriptor.GetEvents(this, attributes, true);

    public PropertyDescriptorCollection GetProperties() => GetProperties([]);

    public PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => new(instance.PropertyDescriptors);

    public object GetPropertyOwner(PropertyDescriptor? pd) => instance;
}
