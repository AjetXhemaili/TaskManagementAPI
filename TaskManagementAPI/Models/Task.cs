// This file intentionally kept to maintain compatibility with existing references.
// The Task model was renamed to TaskItem to avoid conflicts with System.Threading.Tasks.Task.
// Existing code should be updated to use TaskItem. This alias type will help prevent build errors until references are migrated.

namespace TaskManagementAPI.Models
{
    [System.Obsolete("Task has been renamed to TaskItem. Use TaskItem instead.")]
    public class Task : TaskItem
    {
    }
}
