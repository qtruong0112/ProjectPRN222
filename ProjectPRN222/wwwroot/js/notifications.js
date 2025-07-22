"use strict";

// Khởi tạo SignalR - đơn giản như SimpleChat
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

// Bắt đầu connection - giống SimpleChat
connection.start().then(function () {
    console.log("Notification SignalR connected");
    LoadNotifications();
}).catch(function (err) {
    console.error("SignalR connection failed: ", err);
});

// Lắng nghe events từ server - đơn giản
connection.on("ReceiveNotification", function (notification) {
    ShowToast(notification.message);
    LoadNotifications();
    UpdateNotificationBadge();
});

connection.on("LoadNotifications", function () {
    LoadNotifications();
    UpdateNotificationBadge();
});

// Load danh sách notifications - đơn giản
function LoadNotifications() {
    $.get('/Notifications/GetNotifications', function(result) {
        if (result.success) {
            let html = '';
            $.each(result.data, function(i, notification) {
                html += `
                    <div class="p-3 border-bottom notification-item ${!notification.isRead ? 'unread' : ''}" data-id="${notification.id}">
                        <div class="d-flex justify-content-between">
                            <div class="flex-grow-1">
                                ${!notification.isRead ? '<span class="badge bg-success me-2">Mới</span>' : ''}
                                <div>${notification.message}</div>
                                <small class="text-muted">${notification.sentDate}</small>
                            </div>
                            <div class="dropdown">
                                <button class="btn btn-sm btn-light" data-bs-toggle="dropdown">⋮</button>
                                <ul class="dropdown-menu">
                                    ${!notification.isRead ? 
                                        `<li><a class="dropdown-item" onclick="MarkAsRead(${notification.id})">Đánh dấu đã đọc</a></li>` : ''}
                                    <li><a class="dropdown-item text-danger" onclick="DeleteNotification(${notification.id})">Xóa</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                `;
            });
            
            $('#notificationsList').html(html || '<div class="text-center py-5 text-muted">Chưa có thông báo</div>');
        }
    });
}

// Cập nhật badge số lượng - đơn giản
function UpdateNotificationBadge() {
    $.get('/Notifications/GetUnreadCount', function(data) {
        if (data.success) {
            const badge = $('#unreadCountBadge, #headerNotificationBadge');
            if (data.count > 0) {
                badge.text(data.count).show();
            } else {
                badge.hide();
            }
        }
    });
}

// Đánh dấu đã đọc
function MarkAsRead(id) {
    $.post('/Notifications/MarkAsRead', { id: id }, function(result) {
        if (result.success) {
            LoadNotifications();
            UpdateNotificationBadge();
        }
    });
}

// Đánh dấu tất cả đã đọc
function MarkAllAsRead() {
    $.post('/Notifications/MarkAllAsRead', function(result) {
        if (result.success) {
            LoadNotifications();
            UpdateNotificationBadge();
        }
    });
}

// Xóa notification
function DeleteNotification(id) {
    if (confirm('Xóa thông báo này?')) {
        $.ajax({
            url: `/Notifications/Delete/${id}`,
            method: 'DELETE',
            success: function(result) {
                if (result.success) {
                    LoadNotifications();
                    UpdateNotificationBadge();
                }
            }
        });
    }
}

// Hiển thị toast đơn giản
function ShowToast(message) {
    const toast = `
        <div class="toast show" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
            <div class="toast-header">
                <strong class="me-auto">🔔 Thông báo mới</strong>
                <button type="button" class="btn-close" onclick="$(this).closest('.toast').remove()"></button>
            </div>
            <div class="toast-body">${message}</div>
        </div>
    `;
    
    $('body').append(toast);
    
    // Tự động ẩn sau 5 giây
    setTimeout(function() {
        $('.toast').fadeOut(function() { $(this).remove(); });
    }, 5000);
}

// Load khi trang ready
$(document).ready(function() {
    UpdateNotificationBadge();
    
    // Thêm CSS đơn giản
    $('head').append(`
        <style>
            .notification-item.unread {
                background-color: #f8f9fa;
                border-left: 3px solid #007bff;
            }
            .notification-item:hover {
                background-color: #f5f5f5;
            }
        </style>
    `);
}); 