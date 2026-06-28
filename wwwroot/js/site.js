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
    user: null,
    countries: [],
    avatars: [],
    selectedAvatar: 0,
};

const $ = (id) => document.getElementById(id);

const setMessage = (text) => {
    $("sessionBadge").textContent = text;
};

const avatarName = (id) => state.avatars.find((item) => item.id === Number(id))?.name ?? `头像 ${id}`;

const countryName = (code) => state.countries.find((item) => item.code === code)?.name ?? code ?? "--";

const renderOptions = () => {
    $("nationalityInput").innerHTML = state.countries
        .map((item) => `<option value="${escapeHtml(item.code)}">${escapeHtml(item.name)} (${escapeHtml(item.code)})</option>`)
        .join("");

    $("avatarOptions").innerHTML = "";
    for (const avatar of state.avatars) {
        const button = document.createElement("button");
        button.type = "button";
        button.className = "avatar-choice";
        button.dataset.id = avatar.id;
        button.innerHTML = `<span>${avatar.id}</span><small>${escapeHtml(avatar.name)}</small>`;
        button.addEventListener("click", () => {
            state.selectedAvatar = avatar.id;
            renderAvatarSelection();
        });
        $("avatarOptions").appendChild(button);
    }
};

const renderAvatarSelection = () => {
    document.querySelectorAll(".avatar-choice").forEach((button) => {
        button.classList.toggle("selected", Number(button.dataset.id) === Number(state.selectedAvatar));
    });
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
    state.selectedAvatar = Number(user.head || 0);
    setMessage(`${user.name} #${user.userid}`);
    $("avatarBox").textContent = user.head;
    $("avatarBox").title = avatarName(user.head);
    $("playerName").textContent = user.name;
    $("playerMeta").textContent = `${user.openid} · ${countryName(user.country)} · ${user.register || "未知注册时间"}`;
    $("levelValue").textContent = `Lv.${user.level}`;
    $("nextExpValue").textContent = `${user.nextExp} EXP`;
    $("amountValue").textContent = user.amount;
    $("pasValue").textContent = user.pas;
    $("commentValue").textContent = user.comment;
    $("watchedValue").textContent = user.watched;
    $("nicknameInput").value = user.name;
    $("nationalityInput").value = user.country || "";
    renderAvatarSelection();
};

const renderMaps = (maps) => {
    $("mapCountLabel").textContent = `${maps.length} 项`;
    $("mapResults").innerHTML = "";

    if (maps.length === 0) {
        $("mapResults").innerHTML = `<div class="empty-state">没有找到匹配的关卡</div>`;
        return;
    }

    for (const map of maps) {
        const item = document.createElement("article");
        item.className = "map-item";
        item.innerHTML = `
            <div class="map-icon">${escapeHtml(map.icon || "#")}</div>
            <div class="map-main">
                <div class="map-title-row">
                    <h3>${escapeHtml(map.name || "未命名地图")}</h3>
                    <span>#${map.id}</span>
                </div>
                <p>作者：${escapeHtml(map.ownerName || "未知")} · ${escapeHtml(countryName(map.country))} · ${escapeHtml(map.date || "未知日期")}</p>
                <div class="map-stats">
                    <span><strong>${map.amount}</strong> 次游玩</span>
                    <span><strong>${map.pas}</strong> 次通关</span>
                    <span><strong>${formatPassRate(map.passRate)}</strong> 通关率</span>
                    <span><strong>${formatDifficulty(map.difficulty)}</strong>${map.forcedDifficulty ? " 固定难度" : " 动态难度"}</span>
                    <span><strong>${map.exp}</strong> 预计经验</span>
                    <span><strong>${map.like}</strong> 点赞</span>
                    <span><strong>${map.favorite}</strong> 收藏</span>
                </div>
            </div>
        `;
        $("mapResults").appendChild(item);
    }
};

const hydrateBootData = () => {
    state.user = window.boxmakerInitialUser?.ok ? window.boxmakerInitialUser.user : null;
    state.countries = Array.isArray(window.boxmakerCountryOptions) ? window.boxmakerCountryOptions : [];
    state.avatars = Array.isArray(window.boxmakerAvatarOptions) ? window.boxmakerAvatarOptions : [];

    if (!state.countries.some((item) => item.code === "--")) {
        state.countries.unshift({ code: "--", name: "未设置", flag: "gq_000" });
    }

    if (state.avatars.length === 0) {
        state.avatars = [
            { id: 0, name: "游客" },
            { id: 1, name: "兔子" },
            { id: 2, name: "小猪" },
            { id: 3, name: "白鸟" },
            { id: 4, name: "魔花" },
            { id: 5, name: "刺猬" },
            { id: 6, name: "乌龟" },
            { id: 7, name: "空白" },
        ];
    }
};

const formatPassRate = (value) => {
    const number = Number(value || 0);
    return `${Number.isInteger(number) ? number : number.toFixed(1)}%`;
};

const formatDifficulty = (value) => {
    const number = Number(value || 0);
    return number <= 0 ? "未评级" : `难度 ${number}`;
};

const escapeHtml = (value) => String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;");

const refreshSearch = async (query = "") => {
    const data = await api(`/?handler=Search&q=${encodeURIComponent(query)}`);
    renderMaps(data.maps || []);
};

document.addEventListener("DOMContentLoaded", () => {
    hydrateBootData();
    renderOptions();
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
            head: Number(state.selectedAvatar || 0),
        });
        if (!data.ok) {
            setMessage(data.message || "保存失败");
            return;
        }
        state.user = data.user;
        renderUser();
        setMessage("资料已保存");
    });

    $("passwordForm").addEventListener("submit", async (event) => {
        event.preventDefault();
        const data = await api("/?handler=Password", {
            oldPassword: $("oldPasswordInput").value,
            newPassword: $("newPasswordInput").value,
        });
        setMessage(data.message || (data.ok ? "密码已更新" : "更新失败"));
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
