# Generated by Django 5.0 on 2023-12-11 13:10

import django.db.models.deletion
from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('aplikacja', '0005_event_calendarid'),
    ]

    operations = [
        migrations.AlterField(
            model_name='log',
            name='event',
            field=models.ForeignKey(blank=True, null=True, on_delete=django.db.models.deletion.SET_NULL, to='aplikacja.event'),
        ),
    ]
