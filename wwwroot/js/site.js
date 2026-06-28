const api = async (url, payload) => {
    const options = payload === undefined
        ? {}
        : {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        };
    const response = await fetch(url, options);
    return response.json();
};

const state = {
    user: window.boxmakerInitialUser?.ok ? window.boxmakerInitialUser.user : null,
};

const $ = (id) => document.getElementById(id);

const setMessage = (text) => {
    $("sessionBadge").textContent = text;
};

const renderUser = () => {
    const loggedIn = Boolean(state.user);
    $("loginForm").classList.toggle("hidden", loggedIn);
    $("profilePane").classList.toggle("hidden", !loggedIn);
    $("logoutButton").classList.toggle("hidden", !loggedIn);

    if (!loggedIn) {
        setMessage("未登录");
        return;
    }

    const user = state.user;
    setMessage(`${user.name} #${user.userid}`);
    $("avatarBox").textContent = user.head;
    $("playerName").textContent = user.name;
    $("playerMeta").textContent = `${user.openid} · ${user.country || "--"} · ${user.register || ""}`;
    $("levelValue").textContent = user.level;
    $("nextExpValue").textContent = `${user.nextExp} EXP`;
    $("amountValue").textContent = user.amount;
    $("pasValue").textContent = user.pas;
    $("commentValue").textContent = user.comment;
    $("watchedValue").textContent = user.watched;
    $("nicknameInput").value = user.name;
    $("nationalityInput").value = user.country || "";
    $("headInput").value = user.head;
};

const renderMaps = (maps) => {
    $("mapCountLabel").textContent = maps.length;
    $("mapResults").innerHTML = "";

    if (maps.length === 0) {
        $("mapResults").innerHTML = `<div class="empty-state">没有结果</div>`;
        return;
    }

    for (const map of maps) {
        const passRate = map.amount <= 0 ? "0%" : `${Math.round((map.pas / map.amount) * 100)}%`;
        const image = map.url
            ? `<img src="data:image/png;base64,${map.url}" alt="" />`
            : `<div class="map-placeholder">${map.id}</div>`;
        const item = document.createElement("article");
        item.className = "map-item";
        item.innerHTML = `
            <div class="map-thumb">${image}</div>
            <div class="map-info">
                <h3>${escapeHtml(map.name || "未命名地图")}</h3>
                <p>#${map.id}</p>
            </div>
            <div class="map-metrics">
                <span>${map.amount} 玩</span>
                <span>${passRate}</span>
                <span>L${map.difficulty || "-"}</span>
            </div>
        `;
        $("mapResults").appendChild(item);
    }
};

const escapeHtml = (value) => value
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");

const refreshSearch = async (query = "") => {
    const data = await api(`/?handler=Search&q=${encodeURIComponent(query)}`);
    renderMaps(data.maps || []);
};

document.addEventListener("DOMContentLoaded", () => {
    renderUser();
    refreshSearch("");

    $("loginForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = await api("/?handler=Login", {
            openid: $("openidInput").value,
            openkey: $("openkeyInput").value,
        });
        if (!data.ok) {
            setMessage(data.message || "登录失败");
            return;
        }
        state.user = data.user;
        renderUser();
    });

    $("logoutButton").addEventListener("click", async () => {
        await api("/?handler=Logout", {});
        state.user = null;
        renderUser();
    });

    $("profileForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = await api("/?handler=Profile", {
            nickname: $("nicknameInput").value,
            nationality: $("nationalityInput").value,
            head: Number($("headInput").value || 0),
        });
        if (!data.ok) {
            setMessage(data.message || "保存失败");
            return;
        }
        state.user = data.user;
        renderUser();
    });

    $("passwordForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = await api("/?handler=Password", {
            oldPassword: $("oldPasswordInput").value,
            newPassword: $("newPasswordInput").value,
        });
        setMessage(data.message || (data.ok ? "已更新" : "更新失败"));
        if (data.ok) {
            $("oldPasswordInput").value = "";
            $("newPasswordInput").value = "";
        }
    });

    $("searchForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        refreshSearch($("mapSearchInput").value);
    });
});
