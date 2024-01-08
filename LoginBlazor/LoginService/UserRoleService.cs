namespace LoginBlazor2.Security.Services;

using System.Net.Http.Json;
using LoginShared;

public class RoleUserService
{
    //lista de roles
    private readonly List<Role> roleDb;

    //lista de usuarios
    private readonly List<User> userDb;

    public RoleUserService(List<Role> roles, List<User> users)
    {
        // Guardando en las listas los roles y usuarios pasados como argumentos
        this.roleDb = roleDb;
        this.userDb = userDb;
    }
}