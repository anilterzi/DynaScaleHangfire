using Hangfire.Dashboard;
using Hangfire.Dashboard.Pages;

namespace DynaScaleHangfire.Pages;

public class DynamicScalingPage : RazorPage
{
    public override void Execute()
    {
        Layout = new LayoutPage("Dynamic Scaling");

        WriteLiteral(@"
<div class='row'>
    <div class='col-md-12'>
        <h1 class='page-header' style='color: #fff;'>Dynamic Scaling</h1>
        <div class='js-dynamic-scaling'>
            <div class='row'>
                <div class='col-md-12'>
                    <div class='panel panel-default' style='background-color: #2d2d2d; border: 1px solid #444;'>
                        <div class='panel-heading' style='background-color: #1a1a1a; border-bottom: 1px solid #444;'>
                            <h3 class='panel-title' style='color: #fff;'>Queue Worker Counts</h3>
                        </div>
                        <div class='panel-body' style='background-color: #2d2d2d;'>
                            <div class='js-queue-worker-counts'></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<style>
    .table {
        background-color: #2d2d2d;
        color: #fff;
    }
    .table > thead > tr > th {
        background-color: #1a1a1a;
        border-bottom: 1px solid #444;
        color: #fff;
    }
    .table > tbody > tr > td {
        border-top: 1px solid #444;
        color: #fff;
    }
    .table-striped > tbody > tr:nth-of-type(odd) {
        background-color: #333;
    }
    .btn-primary {
        background-color: #375a7f;
        border-color: #375a7f;
    }
    .btn-primary:hover {
        background-color: #2b4764;
        border-color: #2b4764;
    }
    .btn-success {
        background-color: #00bc8c;
        border-color: #00bc8c;
    }
    .btn-success:hover {
        background-color: #009670;
        border-color: #009670;
    }
    .form-control {
        background-color: #1a1a1a;
        border: 1px solid #444;
        color: #fff;
    }
    .form-control:focus {
        background-color: #1a1a1a;
        border-color: #375a7f;
        color: #fff;
    }
    .worker-count-input {
        width: 80px;
        text-align: center;
    }
    .current-worker-count {
        font-weight: bold;
        color: #00bc8c;
    }
</style>

<script src='/js/dynamic-scaling.js'></script>");
    }
}