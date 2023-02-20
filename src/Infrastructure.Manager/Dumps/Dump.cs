using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Infrastructure.Manager.Interfaces;

namespace Infrastructure.Manager.Dumps;

public abstract class Dump<TEntity> : IDump where TEntity : Entity
{
    protected IService<TEntity> Service;

    protected IRepository<TEntity> Repository;

    public int Order { get; }

    public Dump(IService<TEntity> service, IRepository<TEntity> repository, string dumpName, int order = 0)
    {
        Service = service;
        Repository = repository;
        Order = order;

        Console.WriteLine("\t" + dumpName);
    }

    public virtual async Task<bool> CanSaveAsync(TEntity entity)
        => await Task.FromResult(false);

    public virtual async Task Save(TEntity entity)
    {
        try
        {
            if (await CanSaveAsync(entity))
                await Service.AddAsync(entity);
        }
        catch (Exception error)
        {
            PrintError(string.Format("{0} - {1}", error.Message, error.StackTrace));
        }
    }

    public virtual Task DumpAsync() 
        => Task.FromResult(true);

    public virtual void PrintError(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
        Console.ForegroundColor = ConsoleColor.Green;
    }

    public virtual string GetTemplate(string template, string path = "")
    {
        var result = "";
        var filePath = Environment.CurrentDirectory;

        if (!string.IsNullOrEmpty(path))
            filePath += "/Dumps/Templates/" + path + "/" + template;
        else
            filePath += "/Dumps/Templates/" + template;

        if (!File.Exists(filePath)) return result;

        var fileStream = new FileStream(filePath, FileMode.Open);

        using StreamReader reader = new(fileStream);
        result = reader.ReadToEnd();

        return result;
    }
}