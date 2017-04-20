UnityMVP
=================

MVP stands for Model-View-Pool, and is shamelessly based on the MVC methodology, but slightly modified for game development, specially with Unity's MonoBehaviours.

## How to install

The Unity project containing this code is not included (nor the folder structure) so you can add it as a submodule of your repo and update through git more easily. `Plugins` folder is a nice candidate, but anyone will do. It's up to you.

1. `git submodule add https://github.com/L4D15/UnityMVP.git Assets/Plugins/UnityMVP`
2. `git submodule init`
3. `git submodule update`
4. Profit.

## Fundamentals

### Model 

Represents a chunk of data with no logic. A unique identifier is required to distinguish it from other chunks of data of the same type.

Example:

```
public class Animal : IModel
{
    public string Id { get; }

    public Animal(string id)
    {
        Id = id;
    }
}
```

### View

Defines how a model (data) is presented to the user through the game engine. It is associated with a single model element and updates its visuals to represent the model it is associated with.

Example:

```
public AnimalView : View<Animal>
{
    public Text nameLabel;

    protected override void RefreshImplementation(Animal model)
    {
        nameLabel.text = model.Id;
    }

    protected override void CleanImplementation()
    {
        nameLabel.text = string.Empty;
    }
}
```

### Pool

Manages the usage of views during gameplay using pooling techniques for better performance.

Example:

```

public class FooComponent : MonoBehaviour
{
    public AnimalView animalPrefab;

    private void Awake()
    {
        var cat = new Animal("KeyboardCat");

        var pool = new SimplePool<AnimalView>();

        pool.Initialize(animalPrefab, 0);

        var catView = pool.Provide(cat);
    }
}

```


## Where are the controllers?!

There is nothing specific to automate in the controllers, so there is no need to include a base class for them here. Write your own controllers, operate over the model and update the apropriate views as you see fit :-)

Example:

```

public class AnimalController
{
    private SimplePool<AnimalView> _animalViews;

    public void Initialize(SimplePool<AnimalView> pool)
    {
        _animalViews = pool;
    }

    public void Rename(Animal model, string name)
    {
        var animalView = _animalViews.Find(model);

        model.Id = name;

        animalView.Refresh(model);
    }
}

```

## Pro Tips

1. Each model must have a **unique** identifier, otherwise, you may get views of other models when searching or freeing them from the pool.
2. IModel is an interface so you can inherit you model elements from any other base class.
3. Views implement the IUpdatableView<T> and IUpdatableView, so they can be refreshed passing a T element or anything that implements the IModel. 
4. When using the IModel method, if the view does not update, it maybe because the casting from IModel to T failed. Make sure you are passing a model of the right type.
5. Finding or freeing a view from a pool can be done using the model passed in the template, an IModel or even an id.
6. You can iterate over all **used** views in a pool with `foreach (var view in pool)`. Non used views are ignored during the iteration.
7. Remember to free views from the pool when you are no longer using them. They will be reused the next time you ask for a view, saving you from evil `Instatite` operations.
8. When a pool has no more available views to provide, it will instantiate a new one. If you have an estimation on how much views you will need, initialize the pool as `pool.Initialize(prefab, estimatedNumberOfViews)` and that amount of views will be created when initializing the pool.
