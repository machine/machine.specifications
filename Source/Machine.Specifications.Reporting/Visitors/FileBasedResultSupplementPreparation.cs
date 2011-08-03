using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Machine.Specifications.Reporting.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Reporting.Visitors
{
  public class FileBasedResultSupplementPreparation : ISpecificationVisitor
  {
    const string HtmlSupplementPrefix = "html-";
    const string ImageSupplementPrefix = "img-";
    const string TextSupplementPrefix = "text-";
    readonly IFileSystem _fileSystem;
    Func<string> _resourcePathCreator = () => String.Empty;
    string _resourcePath;

    public FileBasedResultSupplementPreparation() : this(new FileSystem())
    {
    }

    public FileBasedResultSupplementPreparation(IFileSystem fileSystem)
    {
      _fileSystem = fileSystem;
    }

    public void Initialize(VisitorContext context)
    {
      _resourcePathCreator = context.ResourcePathCreator;
    }

    public void Visit(Run run)
    {
      run.Assemblies.Each(Visit);
    }

    public void Visit(Assembly assembly)
    {
      assembly.Concerns.Each(Visit);
    }

    public void Visit(Concern concern)
    {
      concern.Contexts.Each(Visit);
    }

    public void Visit(Context context)
    {
      context.Specifications.Each(Visit);
    }

    public void Visit(Specification specification)
    {
      if (!HasSupplements(specification))
      {
        return;
      }

      CreatePathForResources();

      var images = ImageSupplementsOf(specification);
      var html = HtmlSupplementsOf(specification);
      var files = images.Union(html);

      var replaced = files.Select(x =>
        {
          try
          {
            string destinationFileName = CopySupplementToResources(x.ItemValue);
            return x.UpdateItemValue(destinationFileName);
          }
          catch (Exception ex)
          {
            var message = String.Format("Failed to copy supplement {0}\r\n{1}", x.ItemValue, ex);
            var key = TextSupplementPrefix + x.ItemKey + "-error";
            while (specification.Supplements[x.Name].ContainsKey(key))
            {
              key += "-error";
            }
            
            return x.UpdateItemKeyAndValue(key, message);
          }
        });

      replaced.ToList().Each(x =>
        {
          if (x.ItemKey != x.UpdatedItemKey)
          {
            specification.Supplements[x.Name].Remove(x.ItemKey);
            specification.Supplements[x.Name].Add(x.UpdatedItemKey, null);
          }

          specification.Supplements[x.Name][x.UpdatedItemKey] = x.UpdatedItemValue;
        });
    }

    void CreatePathForResources()
    {
      _resourcePath = _resourcePathCreator();
    }

    string CopySupplementToResources(string file)
    {
      var destinationFileName = Path.Combine(_resourcePath, Path.GetFileName(file));
      _fileSystem.Move(file, destinationFileName);
      return destinationFileName;
    }

    static IEnumerable<Supplement> ImageSupplementsOf(Specification specification)
    {
      return SupplementsWithKey(specification, ImageSupplementPrefix);
    }

    static IEnumerable<Supplement> HtmlSupplementsOf(Specification specification)
    {
      return SupplementsWithKey(specification, HtmlSupplementPrefix);
    }

    static IEnumerable<Supplement> SupplementsWithKey(Specification specification, string key)
    {
      return specification.Supplements.Keys
        .SelectMany(name => specification
                              .Supplements.SelectMany(x => x.Value)
                              .Where(x => x.Key.StartsWith(key))
                              .Select(x => new Supplement { Name = name, ItemKey = x.Key, ItemValue = x.Value }));
    }

    static bool HasSupplements(Specification specification)
    {
      return specification.Supplements.Any();
    }

    #region Nested type: Supplement
    class Supplement
    {
      public string Name
      {
        get;
        set;
      }

      public string ItemKey
      {
        get;
        set;
      }

      public string ItemValue
      {
        get;
        set;
      }

      public string UpdatedItemKey
      {
        get;
        set;
      }

      public string UpdatedItemValue
      {
        get;
        set;
      }

      public Supplement UpdateItemValue(string newValue)
      {
        return UpdateItemKeyAndValue(ItemKey, newValue);
      }

      public Supplement UpdateItemKeyAndValue(string newKey, string newValue)
      {
        return new Supplement
               {
                 Name = Name,
                 ItemKey = ItemKey,
                 ItemValue = ItemValue,
                 UpdatedItemKey = newKey,
                 UpdatedItemValue = newValue
               };
      }
    }
    #endregion
  }
}