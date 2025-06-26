document.addEventListener('DOMContentLoaded', function() {
    var $ = window.jQuery;
    
    function updateQueueWorkerCounts() {
        $.get('/dynamic-scaling/queues', function(data) {
            var html = '<table class="table table-striped">';
            html += '<thead><tr><th>Queue</th><th>Current Worker Count</th><th>New Worker Count</th><th>Actions</th></tr></thead>';
            html += '<tbody>';
            
            data.forEach(function(queue) {
                html += '<tr>';
                html += '<td>' + queue.name + '</td>';
                html += '<td><span class="current-worker-count">' + queue.workerCount + '</span></td>';
                html += '<td><input type="number" class="form-control worker-count-input" value="' + queue.workerCount + '" min="1" max="100"></td>';
                html += '<td>';
                html += '<button class="btn btn-success btn-sm js-save-workers" data-queue="' + queue.name + '">Save</button>';
                html += '</td>';
                html += '</tr>';
            });
            
            html += '</tbody></table>';
            $('.js-queue-worker-counts').html(html);
        });
    }

    $(document).on('click', '.js-save-workers', function() {
        var $button = $(this);
        var queueName = $button.data('queue');
        var $row = $button.closest('tr');
        var newWorkerCount = $row.find('.worker-count-input').val();
        
        $.ajax({
            url: '/dynamic-scaling/queues/' + queueName + '/set-workers',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ workerCount: parseInt(newWorkerCount) }),
            success: function() {
                $row.find('.current-worker-count').text(newWorkerCount);
                $button.removeClass('btn-success').addClass('btn-default').text('Saved');
                setTimeout(function() {
                    $button.removeClass('btn-default').addClass('btn-success').text('Save');
                }, 2000);
            }
        });
    });

    updateQueueWorkerCounts();
    setInterval(updateQueueWorkerCounts, 30000);
}); 