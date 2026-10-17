
const API_URL = "http://localhost:8080/tasks";

const taskForm = document.getElementById("task-form");
const taskTitle = document.getElementById("task-title");
const taskList = document.getElementById("task-list");
const taskCount = document.getElementById("task-count");
const message = document.getElementById("message");

async function loadTasks() {
    try {
        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error("Could not load tasks.");
        }

        const tasks = await response.json();
        renderTasks(tasks);
        message.textContent = "";
    } catch (error) {
        message.textContent =
            "Could not connect to the API. Check that Docker is running.";
        taskCount.textContent = "Tasks unavailable";
    }
}

function renderTasks(tasks) {
    taskList.replaceChildren();

    taskCount.textContent =
        `${tasks.length} task${tasks.length === 1 ? "" : "s"}`;

    for (const task of tasks) {
        const item = document.createElement("li");
        item.className = "task-item";

        if (task.isCompleted) {
            item.classList.add("completed");
        }

        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.checked = task.isCompleted;
        checkbox.setAttribute("aria-label", `Complete ${task.title}`);

        checkbox.addEventListener("change", async () => {
            await updateTask(task.id, task.title, checkbox.checked);
        });

        const title = document.createElement("span");
        title.className = "task-title";
        title.textContent = task.title;

        const actions = document.createElement("div");
        actions.className = "task-actions";

        const editButton = document.createElement("button");
        editButton.type = "button";
        editButton.textContent = "Edit";

        editButton.addEventListener("click", async () => {
            const newTitle = prompt("Edit task title:", task.title);

            if (newTitle === null) return;

            if (!newTitle.trim()) {
                message.textContent = "Task title cannot be empty.";
                return;
            }

            await updateTask(task.id, newTitle.trim(), task.isCompleted);
        });

        const deleteButton = document.createElement("button");
        deleteButton.type = "button";
        deleteButton.textContent = "Delete";
        deleteButton.className = "delete-button";

        deleteButton.addEventListener("click", async () => {
            if (!confirm(`Delete "${task.title}"?`)) return;
            await deleteTask(task.id);
        });

        actions.append(editButton, deleteButton);
        item.append(checkbox, title, actions);
        taskList.appendChild(item);
    }
}

taskForm.addEventListener("submit", async (event) => {
    event.preventDefault();

    const title = taskTitle.value.trim();

    if (!title) {
        message.textContent = "Please enter a task title.";
        return;
    }

    try {
        const response = await fetch(API_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                title: title,
                isCompleted: false
            })
        });

        if (!response.ok) {
            throw new Error("Could not create task.");
        }

        taskTitle.value = "";
        message.textContent = "Task added successfully.";
        await loadTasks();
    } catch (error) {
        message.textContent = error.message;
    }
});

async function updateTask(id, title, isCompleted) {
    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ title, isCompleted })
        });

        if (!response.ok) {
            throw new Error("Could not update task.");
        }

        message.textContent = "Task updated successfully.";
        await loadTasks();
    } catch (error) {
        message.textContent = error.message;
        await loadTasks();
    }
}

async function deleteTask(id) {
    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: "DELETE"
        });

        if (!response.ok) {
            throw new Error("Could not delete task.");
        }

        message.textContent = "Task deleted successfully.";
        await loadTasks();
    } catch (error) {
        message.textContent = error.message;
    }
}

loadTasks();