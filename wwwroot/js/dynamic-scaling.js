document.addEventListener('DOMContentLoaded', function() {
    var $ = window.jQuery;
    
    function updateServerManagement() {
        $.get('/dynamic-scaling/servers', function(data) {
            var html = '';
            
            data.forEach(function(server) {
                var statusClass = server.isActive ? 'active' : 'inactive';
                var statusText = server.isActive ? 'Active' : 'Inactive';
                var lastHeartbeat = new Date(server.lastHeartbeat).toLocaleString();
                
                html += '<div class="server-header js-server-header" data-server="' + server.serverName + '">';
                html += '<div class="server-name">' + server.serverName + '</div>';
                html += '<div class="server-details">';
                html += '<span class="server-status ' + statusClass + '">' + statusText + '</span>';
                html += ' | Last Heartbeat: ' + lastHeartbeat;
                html += ' | Queues: ' + server.queues.length;
                html += '</div>';
                html += '</div>';
                
                html += '<div class="queue-table js-queue-table" data-server="' + server.serverName + '" style="display: none;">';
                
                if (server.queues.length === 0) {
                    html += '<div class="queue-row">';
                    html += '<div class="queue-name">No queues configured</div>';
                    html += '</div>';
                } else {
                    server.queues.forEach(function(queue) {
                        html += '<div class="queue-row">';
                        html += '<div class="queue-name">' + queue.queueName + '</div>';
                        html += '<div class="queue-details">';
                        html += 'Current Workers: <span class="current-worker-count">' + queue.currentWorkerCount + '</span>';
                        html += ' | Max Workers: ' + queue.maxWorkerCount;
                        html += '</div>';
                        html += '<div style="margin-top: 10px;">';
                        html += '<input type="number" class="form-control worker-count-input" value="' + queue.currentWorkerCount + '" min="1" max="100" style="display: inline-block; width: 100px; margin-right: 10px;">';
                        html += '<div class="btn-group">';
                        html += '<button class="btn btn-success btn-sm js-save-workers" data-server-name="' + queue.serverName + '" data-queue="' + queue.queueName + '" data-apply-all="false">This Server</button>';
                        html += '<button class="btn btn-warning btn-sm js-save-workers" data-server-name="' + queue.serverName + '" data-queue="' + queue.queueName + '" data-apply-all="true">All Servers</button>';
                        html += '</div>';
                        html += '</div>';
                        html += '</div>';
                    });
                }
                
                html += '</div>';
            });
            
            $('.js-server-management').html(html);
        });
    }

    $(document).on('click', '.js-server-header', function() {
        var serverName = $(this).data('server');
        var $queueTable = $('.js-queue-table[data-server="' + serverName + '"]');
        
        if ($queueTable.is(':visible')) {
            $queueTable.slideUp();
        } else {
            $queueTable.slideDown();
        }
    });

    $(document).on('click', '.js-save-workers', function() {
        var $button = $(this);
        var serverName = $button.data('server-name');
        var queueName = $button.data('queue');
        var applyToAll = $button.data('apply-all') === true;
        var $queueRow = $button.closest('.queue-row');
        var newWorkerCount = $queueRow.find('.worker-count-input').val();
        
        // Butonları devre dışı bırak
        $queueRow.find('.js-save-workers').prop('disabled', true);
        
        $.ajax({
            url: '/dynamic-scaling/servers/' + serverName + '/queues/' + queueName + '/set-workers',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ 
                workerCount: parseInt(newWorkerCount),
                applyToAllServers: applyToAll
            }),
            success: function() {
                $queueRow.find('.current-worker-count').text(newWorkerCount);
                $button.removeClass('btn-success btn-warning').addClass('btn-default').text('Saved');
                
                setTimeout(function() {
                    $button.removeClass('btn-default');
                    if (applyToAll) {
                        $button.addClass('btn-warning').text('All Servers');
                    } else {
                        $button.addClass('btn-success').text('This Server');
                    }
                    $queueRow.find('.js-save-workers').prop('disabled', false);
                    
                    // Sayfayı yenile
                    window.location.reload();
                }, 2000);
            },
            error: function() {
                $button.removeClass('btn-success btn-warning').addClass('btn-danger').text('Error');
                
                setTimeout(function() {
                    $button.removeClass('btn-danger');
                    if (applyToAll) {
                        $button.addClass('btn-warning').text('All Servers');
                    } else {
                        $button.addClass('btn-success').text('This Server');
                    }
                    $queueRow.find('.js-save-workers').prop('disabled', false);
                }, 2000);
            }
        });
    });

    updateServerManagement();
    setInterval(updateServerManagement, 30000);
}); 