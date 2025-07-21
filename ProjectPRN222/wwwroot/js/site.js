// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// =========================
// NOTIFICATION MANAGEMENT
// =========================
// Quản lý thông báo cho trang Notifications
// Tự động sử dụng SignalR connection từ header nếu có để tránh xung đột

let notificationConnection;
let notificationCurrentPage = 0;

// Khởi tạo SignalR connection cho notifications
function initializeNotificationSignalR() {
    // Sử dụng connection từ header nếu có, nếu không thì tạo mới
    if (typeof headerConnection !== 'undefined' && headerConnection) {
        notificationConnection = headerConnection;
    } else {
        notificationConnection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        notificationConnection.start().then(function () {
            console.log('Notification SignalR Connected');
        }).catch(function (err) {
            console.error('Notification SignalR Connection Error: ', err);
        });
    }

    // Lắng nghe thông báo mới cho trang notifications
    notificationConnection.on("ReceiveNotification", function (notification) {
        showNotificationToast(notification);
        addNotificationToList(notification);
        updateUnreadCount();
    });
}

// Hiển thị toast notification
function showNotificationToast(notification) {
    const toastTitle = document.getElementById('toastTitle');
    const toastMessage = document.getElementById('toastMessage');
    const toastTime = document.getElementById('toastTime');
    
    if (toastTitle && toastMessage && toastTime) {
        toastTitle.textContent = notification.title || 'Thông báo';
        toastMessage.textContent = notification.message;
        toastTime.textContent = notification.sentDate;
        
        const toast = new bootstrap.Toast(document.getElementById('notificationToast'));
        toast.show();
    }
}

// Thêm notification vào danh sách
function addNotificationToList(notification) {
    const notificationsList = document.getElementById('notificationsList');
    if (notificationsList) {
        const notificationItem = createNotificationElement(notification);
        
        // Thêm vào đầu danh sách
        if (notificationsList.firstChild) {
            notificationsList.insertBefore(notificationItem, notificationsList.firstChild);
        } else {
            notificationsList.appendChild(notificationItem);
        }
    }
}

// Tạo element notification
function createNotificationElement(notification) {
    const div = document.createElement('div');
    div.className = 'notification-item p-3 border-bottom ' + (notification.isRead ? 'read' : 'unread');
    div.setAttribute('data-id', notification.id);
    
    div.innerHTML = `
        <div class="d-flex justify-content-between align-items-start">
            <div class="flex-grow-1">
                <div class="d-flex align-items-center mb-2">
                    ${!notification.isRead ? '<span class="badge bg-success me-2">Mới</span>' : ''}
                    <small class="text-muted">${notification.sentDate}</small>
                </div>
                <p class="mb-0 notification-message">${notification.message}</p>
            </div>
            <div class="dropdown">
                <button class="btn btn-sm btn-light" type="button" data-bs-toggle="dropdown">
                    <i class="bi bi-three-dots-vertical"></i>
                </button>
                <ul class="dropdown-menu">
                    ${!notification.isRead ? 
                        `<li><a class="dropdown-item" href="#" onclick="markAsRead(${notification.id})">
                            <i class="bi bi-check"></i> Đánh dấu đã đọc
                        </a></li>` : ''}
                    <li><a class="dropdown-item text-danger" href="#" onclick="deleteNotification(${notification.id})">
                        <i class="bi bi-trash"></i> Xóa
                    </a></li>
                </ul>
            </div>
        </div>
    `;
    
    return div;
}

// Đánh dấu notification đã đọc
function markAsRead(notificationId) {
    fetch(`/Notifications/MarkAsRead`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `id=${notificationId}`
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const notificationItem = document.querySelector(`[data-id="${notificationId}"]`);
            if (notificationItem) {
                notificationItem.classList.remove('unread');
                notificationItem.classList.add('read');
                const badge = notificationItem.querySelector('.badge');
                if (badge) badge.remove();
            }
            updateUnreadCount();
        }
    });
}

// Đánh dấu tất cả đã đọc
function markAllAsRead() {
    fetch('/Notifications/MarkAllAsRead', {
        method: 'POST'
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            document.querySelectorAll('.notification-item.unread').forEach(item => {
                item.classList.remove('unread');
                item.classList.add('read');
                const badge = item.querySelector('.badge');
                if (badge) badge.remove();
            });
            updateUnreadCount();
        }
    });
}

// Xóa notification
function deleteNotification(notificationId) {
    if (confirm('Bạn có chắc muốn xóa thông báo này?')) {
        fetch(`/Notifications/Delete/${notificationId}`, {
            method: 'DELETE'
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const notificationItem = document.querySelector(`[data-id="${notificationId}"]`);
                if (notificationItem) {
                    notificationItem.remove();
                }
                updateUnreadCount();
            }
        });
    }
}

// Cập nhật số lượng thông báo chưa đọc
function updateUnreadCount() {
    fetch('/Notifications/GetUnreadCount')
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const unreadCountBadge = document.getElementById('unreadCountBadge');
                if (unreadCountBadge) {
                    unreadCountBadge.textContent = data.count;
                }
            }
        });
}

// Làm mới danh sách thông báo
function refreshNotifications() {
    location.reload();
}

// Tải thêm thông báo
function loadMoreNotifications() {
    notificationCurrentPage++;
    fetch(`/Notifications/GetNotifications?skip=${notificationCurrentPage * 20}&take=20`)
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const notificationsList = document.getElementById('notificationsList');
                if (notificationsList) {
                    data.data.forEach(notification => {
                        const notificationItem = createNotificationElement(notification);
                        notificationsList.appendChild(notificationItem);
                    });
                }
            }
        });
}

// Xử lý form tạo thông báo
function handleCreateNotificationForm() {
    const notificationForm = document.getElementById('notificationForm');
    if (notificationForm) {
        notificationForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const data = {
                UserId: parseInt(document.getElementById('userId').value),
                Title: document.getElementById('title').value,
                Message: document.getElementById('message').value
            };
            
            fetch('/Notifications/Create', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            })
            .then(response => response.json())
            .then(data => {
                const resultDiv = document.getElementById('result');
                if (resultDiv) {
                    if (data.success) {
                        resultDiv.innerHTML = '<div class="alert alert-success">Tạo thông báo thành công!</div>';
                        notificationForm.reset();
                    } else {
                        resultDiv.innerHTML = '<div class="alert alert-danger">Lỗi: ' + data.message + '</div>';
                    }
                }
            })
            .catch(error => {
                const resultDiv = document.getElementById('result');
                if (resultDiv) {
                    resultDiv.innerHTML = '<div class="alert alert-danger">Lỗi kết nối: ' + error.message + '</div>';
                }
            });
        });
    }
}

// Khởi tạo khi DOM loaded
document.addEventListener('DOMContentLoaded', function() {
    // Khởi tạo SignalR nếu đang ở trang notifications
    if (window.location.pathname.includes('/Notifications')) {
        initializeNotificationSignalR();
    }
    
    // Khởi tạo form handler
    handleCreateNotificationForm();
});
