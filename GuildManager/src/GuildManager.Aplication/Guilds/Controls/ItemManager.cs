using System.Text.Json;
using System.Text.Json.Nodes;
using System;
using System.IO;

namespace GuildManager.Aplication.Guilds.Controls;

public class ItemManager
{
    public List<Item> itemsShop;
    public List<Item> inventory;

    public ItemManager()
    {
        this.itemsShop = new List<Item>();
        this.inventory = new List<Item>();
    }

    public void refreshShop(int prestige)
{
    List<Item> items = generateItemsFromJson();
    //Console.WriteLine($"generateItemsFromJson a retourné {items.Count} items");
    //Console.WriteLine($"prestige = {prestige}, seuil = {prestige/10 + 1}");

    this.itemsShop.Clear();
    foreach(Item item in items)
    {
        Console.WriteLine($"  item {item.name} : lvl={item.lvl}");
        if (item.lvl >= prestige/10 + 1)
        {
            this.itemsShop.Add(item);
        }
    }
    //Console.WriteLine($"itemsShop contient maintenant {this.itemsShop.Count} items");
}

    public void refreshInventory()
    {
        string json = File.ReadAllText("data/inventory.json");
        JsonDocument doc = JsonDocument.Parse(json);
        JsonElement adventurersJson = doc.RootElement.GetProperty("items");
        List<Item> items = JsonSerializer.Deserialize<List<Item>>(adventurersJson.GetRawText()) ?? new List<Item>();

        this.inventory = items;
    }

    public List<Item> generateItemsFromJson()
    {
        string itemsJson = File.ReadAllText("data/items.json");
        List<Item> items = JsonSerializer.Deserialize<List<Item>>(itemsJson) ?? new List<Item>();
        //Console.WriteLine($"Items lus depuis items.json : {items.Count}");

        string invJson = File.ReadAllText("data/inventory.json");
        JsonObject root = JsonNode.Parse(invJson)!.AsObject();
        int idCount = root["idCount"]?.GetValue<int>() ?? 0;

        foreach (Item item in items)
        {
            item.id = idCount;
            idCount++;
        }

        root["idCount"] = idCount;
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText("data/inventory.json", root.ToJsonString(options));

        return items;
    }

    public void addItem(Item item)
    {
        this.inventory.Add(item);
        addItemToJson(item);
    }

    public void addItemToJson(Item item)
    {
        string json = File.ReadAllText("data/inventory.json");
        JsonObject root = JsonNode.Parse(json)!.AsObject();


        var newItem = new JsonObject
        {
            ["id"] = item.id,
            ["name"] = item.name,
            ["type"] = item.type,
            ["lvl"] = item.lvl,
            ["health"] = item.health,
            ["physicAttack"] = item.physicAttack,
            ["magicAttack"] = item.magicAttack,
            ["def"] = item.def,
            ["image"] = item.image,
            ["goldPrice"] = item.goldPrice,
        };

        if (root["items"] is not JsonArray items)
        {
            items = new JsonArray();
            root["adventurers"] = items;
        }

        items.Add(newItem);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText("data/inventory.json", root.ToJsonString(options));
    }
}